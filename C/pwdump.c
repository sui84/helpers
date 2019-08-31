/*
 * (C) Jeremy Allison 1997. All rights reserved.
 * 
 * This program is free for commercial and non-commercial use.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted.
 *
 * THIS SOFTWARE IS PROVIDED BY JEREMY ALLISON ``AS IS'' AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 *
 */

#include <windows.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>

#include "des.h"

/*
 * Program to dump the Lanman and NT MD4 Hashed passwords from
 * an NT SAM database into a Samba smbpasswd file. Needs Administrator 
 * privillages to run.
 * Takes one arg - the name of the machine whose SAM database you
 * wish to dump, if this arg is not given it dumps the local machine
 * account database.
 */

/*
 * Convert system error to char. Returns 
 * memory allocated with LocalAlloc.
 */

char *error_to_string(DWORD error)
{
  char *msgbuf;
  
  if(FormatMessage(
       FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
       NULL,
       error,
       MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), /* Default language */
       (char *)&msgbuf,
       0,
       NULL
       ) == 0)
    return 0;
  return msgbuf;
}

/*
 * Return a pointer to a string describing an os error.
 * error_to_string returns a pointer to LocalAlloc'ed
 * memory. Cache it and release when the next one is
 * requested.
 */

char *str_oserr(DWORD err)
{
  static char *lastmsg = 0;

  if(lastmsg)
    LocalFree((HLOCAL)lastmsg);

  lastmsg = error_to_string(err);
  return lastmsg;
}

/*
 * Utility function to get allocate a SID from a name.
 * Looks on local machine. SID is allocated with LocalAlloc
 * and must be freed by the caller.
 * Returns TRUE on success, FALSE on fail.
 */

BOOL get_sid(const char *name, SID **ppsid)
{
  SID_NAME_USE sid_use;
  DWORD sid_size = 0;
  DWORD dom_size = 0;
  char *domain;

  *ppsid = 0;
  if(LookupAccountName(0, name, 0, &sid_size, 0, &dom_size, &sid_use) == 0) {
    if(GetLastError() != ERROR_INSUFFICIENT_BUFFER) {
      fprintf( stderr, "get_sid: LookupAccountName for size on name %s failed. Error was %s\n",
            name, str_oserr(GetLastError()));
      return FALSE;
    }
  }

  *ppsid = (SID *)LocalAlloc( LMEM_FIXED, sid_size);
  domain = (char *)LocalAlloc( LMEM_FIXED, dom_size);
  if( *ppsid == 0 || domain == 0) {
    fprintf( stderr, "get_sid: LocalAlloc failed. Error was %s\n",
                 str_oserr(GetLastError()));
    if(*ppsid)
      LocalFree((HLOCAL)*ppsid);
    if(domain)
      LocalFree((HLOCAL)domain);
    *ppsid = 0;
    return FALSE;
  }

  if(LookupAccountName(0, name, *ppsid, &sid_size, domain, &dom_size, &sid_use) == 0) {
    fprintf( stderr, 
         "get_sid: LookupAccountName failed for name %s. Error was %s\n",
         name, str_oserr(GetLastError()));
    LocalFree((HLOCAL)*ppsid);
    LocalFree((HLOCAL)domain);
    *ppsid = 0;
    return FALSE;
  }

  LocalFree((HLOCAL)domain);
  return TRUE;
}

/*
 * Utility function to setup a security descriptor
 * from a varargs list of char *name followed by a DWORD access
 * mask. The access control list is allocated with LocalAlloc
 * and must be freed by the caller.
 * returns TRUE on success, FALSE on fail.
 */

BOOL create_sd_from_list( SECURITY_DESCRIPTOR *sdout, int num, ...)
{
  va_list ap;
  SID **sids = 0;
  char *name;
  DWORD amask;
  DWORD acl_size;
  PACL pacl = 0;
  int i;

  if((sids = (SID **)calloc(1,sizeof(SID *)*num)) == 0) {
    fprintf(stderr, "create_sd_from_list: calloc fail.\n");
    return FALSE;
  }

  acl_size = num * (sizeof(ACL) +
             sizeof(ACCESS_ALLOWED_ACE) +
             sizeof(DWORD));

  /* Collect all the SID's */
  va_start( ap, num);
  for( i = 0; i < num; i++) {
    name = va_arg( ap, char *);
    amask = va_arg(ap, DWORD);
    if(get_sid( name, &sids[i]) == FALSE)
      goto cleanup;
    acl_size += GetLengthSid(sids[i]);
  }
  va_end(ap);
  if((pacl = (PACL)LocalAlloc( LMEM_FIXED, acl_size)) == 0) {
    fprintf( stderr, "create_sd_from_list: LocalAlloc fail. Error was %s\n",
            str_oserr(GetLastError()));
    goto cleanup;
  }

  if(InitializeSecurityDescriptor( sdout, SECURITY_DESCRIPTOR_REVISION) == FALSE) {
    fprintf( stderr, "create_sd_from_list: InitializeSecurityDescriptor fail. Error was %s\n",
                 str_oserr(GetLastError()));
    goto cleanup;
  }
  if(InitializeAcl( pacl, acl_size, ACL_REVISION) == FALSE) {
    fprintf( stderr, "create_sd_from_list: InitializeAcl fail. Error was %s\n",
                 str_oserr(GetLastError()));
    goto cleanup;
  }
  va_start(ap, num);
  for( i = 0; i < num; i++) {
    ACE_HEADER *ace_p;
    name = va_arg( ap, char *);
    amask = va_arg( ap, DWORD);
    if(AddAccessAllowedAce( pacl, ACL_REVISION, amask, sids[i]) == FALSE) {
      fprintf( stderr, "create_sd_from_list: AddAccessAllowedAce fail. Error was %s\n",
                 str_oserr(GetLastError()));
      goto cleanup;
    }
    /* Make sure the ACE is inheritable */
    if(GetAce( pacl, 0, (LPVOID *)&ace_p) == FALSE) {
      fprintf( stderr, "create_sd_from_list: GetAce fail. Error was %s\n",
                 str_oserr(GetLastError()));
      goto cleanup;
    }
    ace_p->AceFlags |= ( CONTAINER_INHERIT_ACE | OBJECT_INHERIT_ACE);
  }

  /* Add the ACL into the sd. */
  if(SetSecurityDescriptorDacl( sdout, TRUE, pacl, FALSE) == FALSE) {
    fprintf( stderr, "create_sd_from_list: SetSecurityDescriptorDacl fail. Error was %s\n",
               str_oserr(GetLastError()));
    goto cleanup;
  }
  for( i = 0; i < num; i++)
    if(sids[i] != 0)
      LocalFree((HLOCAL)sids[i]);
  free(sids);

  return TRUE;

cleanup:

  if(sids != 0) {
    for( i = 0; i < num; i++)
      if(sids[i] != 0)
        LocalFree((HLOCAL)sids[i]);
    free(sids);
  }
  if(pacl != 0)
    LocalFree((HLOCAL)pacl);
  return FALSE;
}

/*
 * Function to go over all the users in the SAM and set an ACL
 * on them.
 */

int set_userkeys_security( HKEY start, const char *path, SECURITY_DESCRIPTOR *psd, 
						  HKEY *return_key)
{
	HKEY key;
	DWORD err;
	char usersid[128];
	DWORD indx = 0;
	
	/* Open the path and enum all the user keys - setting
	   the same security on them. */
	if((err = RegOpenKeyEx( start, path, 0, KEY_ENUMERATE_SUB_KEYS, &key)) !=
					ERROR_SUCCESS) {
		fprintf(stderr, "set_userkeys_security: Failed to open key %s to enumerate. \
Error was %s.\n",
				    path, str_oserr(err));
			return -1;
	}


	/* Now enumerate the subkeys, setting the security on them all. */
	do {
		DWORD size;
		FILETIME ft;

		size = sizeof(usersid);
		err = RegEnumKeyEx(	key, indx, usersid, &size, 0, 0, 0, &ft);
		if(err == ERROR_SUCCESS) {
			HKEY subkey;

			indx++;
			if((err = RegOpenKeyEx( key, usersid, 0, WRITE_DAC, &subkey)) !=
						ERROR_SUCCESS) {
				fprintf(stderr, "set_userkeys_security: Failed to open key %s to set security. \
Error was %s.\n",
						usersid, str_oserr(err));
				RegCloseKey(key);
				return -1;
			}
			if((err = RegSetKeySecurity( subkey, DACL_SECURITY_INFORMATION,
										 psd)) != ERROR_SUCCESS) {
				fprintf(stderr, "set_userkeys_security: Failed to set security on key %s. \
Error was %s.\n",
						usersid, str_oserr(err));
				RegCloseKey(subkey);
				RegCloseKey(key);
				return -1;
			}
			RegCloseKey(subkey);
		}
	} while(err == ERROR_SUCCESS);

	if(err != ERROR_NO_MORE_ITEMS) {
		RegCloseKey(key);
		return -1;
	}
	if(return_key == 0)
		RegCloseKey(key);
	else
		*return_key = key;
	return 0;
}

/*
 * Function to travel down the SAM security tree in the registry and restore
 * the correct ACL on them. Returns 0 on success. -1 on fail.
 */

int restore_sam_tree_access( HKEY start )
{
	char path[128];
	char *p;
	HKEY key;
	DWORD err;
	SECURITY_DESCRIPTOR sd;
	DWORD admin_mask;

	admin_mask = WRITE_DAC | READ_CONTROL;

	if(create_sd_from_list( &sd, 2, "SYSTEM", GENERIC_ALL,
							"Administrators", admin_mask) == FALSE)
		return -1;

	strcpy( path, "SECURITY\\SAM\\Domains\\Account\\Users");

	/* Remove the security on the user keys first. */
	if(set_userkeys_security( start, path, &sd, 0) != 0)
			return -1;

	/* now go up the path, restoring security */
	do {
		if((err = RegOpenKeyEx( start, path, 0, WRITE_DAC, &key)) !=
						ERROR_SUCCESS) {
			fprintf(stderr, "restore_sam_tree_access:Failed to open key %s to set \
security. Error was %s.\n",
					path, str_oserr(err));
			return -1;
		}
		if((err = RegSetKeySecurity( key, DACL_SECURITY_INFORMATION,
									 &sd)) != ERROR_SUCCESS) {
			fprintf(stderr, "restore_sam_tree_access: Failed to set security on key %s. \
Error was %s.\n",
					path, str_oserr(err));
			RegCloseKey(key);
			return  -1;
		}
		RegCloseKey(key);
		p = strrchr(path, '\\');
		if( p != 0) 
			*p = 0;
	} while( p != 0 );

	return 0;
}

/*
 * Function to travel the security tree and add Administrators
 * access as WRITE_DAC, READ_CONTROL and READ.
 * Returns 0 on success. -1 on fail if no security was changed,
 * -2 on fail if security was changed.
 */

int set_sam_tree_access( HKEY start, HKEY *return_key)
{
	char path[128];
	char *p;
	HKEY key;
	DWORD err;
	BOOL security_changed = FALSE;
	SECURITY_DESCRIPTOR sd;
	DWORD admin_mask;
	BOOL finished = FALSE;

	admin_mask = WRITE_DAC | READ_CONTROL | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS;

	if(create_sd_from_list( &sd, 2, "SYSTEM", GENERIC_ALL,
							"Administrators", admin_mask) == FALSE)
		return -1;

	strcpy( path, "SECURITY\\SAM\\Domains\\Account\\Users");
	p = strchr(path, '\\');

	do {
		if( p != 0) 
			*p = 0;
		else
			finished = TRUE;
		if((err = RegOpenKeyEx( start, path, 0, WRITE_DAC, &key)) !=
						ERROR_SUCCESS) {
			fprintf(stderr, "set_sam_tree_access:Failed to open key %s to set \
security. Error was %s.\n",
					path, str_oserr(err));
			return (security_changed ? -2: -1);
		}
		if((err = RegSetKeySecurity( key, DACL_SECURITY_INFORMATION,
									 &sd)) != ERROR_SUCCESS) {
			fprintf(stderr, "set_sam_tree_access: Failed to set security on key %s. \
Error was %s.\n",
					path, str_oserr(err));
			RegCloseKey(key);
			return (security_changed ? -2: -1);
		}
		security_changed = TRUE;
		RegCloseKey(key);
		if(p != 0) {
			*p++ = '\\';
			p = strchr(p, '\\');
		}
	} while( !finished );

	if(set_userkeys_security( start, path, &sd, &key) != 0)
		return -2;
	if(return_key == 0)
		RegCloseKey(key);
	else
		*return_key = key;
	return 0;
}

/* 
 * Function to get a little-endian int from an offset into
 * a byte array.
 */

int get_int( char *array )
{
	return ((array[0]&0xff) + ((array[1]<<8)&0xff00) +
		   ((array[2]<<16)&0xff0000) +
		   ((array[3]<<24)&0xff000000));
}

/*
 * Convert a 7 byte array into an 8 byte des key with odd parity.
 */

void str_to_key(unsigned char *str,unsigned char *key)
{
	void des_set_odd_parity(des_cblock *);
	int i;

	key[0] = str[0]>>1;
	key[1] = ((str[0]&0x01)<<6) | (str[1]>>2);
	key[2] = ((str[1]&0x03)<<5) | (str[2]>>3);
	key[3] = ((str[2]&0x07)<<4) | (str[3]>>4);
	key[4] = ((str[3]&0x0F)<<3) | (str[4]>>5);
	key[5] = ((str[4]&0x1F)<<2) | (str[5]>>6);
	key[6] = ((str[5]&0x3F)<<1) | (str[6]>>7);
	key[7] = str[6]&0x7F;
	for (i=0;i<8;i++) {
		key[i] = (key[i]<<1);
	}
	des_set_odd_parity((des_cblock *)key);
}

/*
 * Function to convert the RID to the first decrypt key.
 */

void sid_to_key1(unsigned long sid,unsigned char deskey[8])
{
	unsigned char s[7];

	s[0] = (unsigned char)(sid & 0xFF);
	s[1] = (unsigned char)((sid>>8) & 0xFF);
	s[2] = (unsigned char)((sid>>16) & 0xFF);
	s[3] = (unsigned char)((sid>>24) & 0xFF);
	s[4] = s[0];
	s[5] = s[1];
	s[6] = s[2];

	str_to_key(s,deskey);
}

/*
 * Function to convert the RID to the second decrypt key.
 */

void sid_to_key2(unsigned long sid,unsigned char deskey[8])
{
	unsigned char s[7];
	
	s[0] = (unsigned char)((sid>>24) & 0xFF);
	s[1] = (unsigned char)(sid & 0xFF);
	s[2] = (unsigned char)((sid>>8) & 0xFF);
	s[3] = (unsigned char)((sid>>16) & 0xFF);
	s[4] = s[0];
	s[5] = s[1];
	s[6] = s[2];

	str_to_key(s,deskey);
}

/*
 * Function to split a 'V' entry into a users name, passwords and comment.
 */

int check_vp(char *vp, int vp_size, char **username, char **fullname,
			 char **comment, char **homedir,
			 char *lanman,int *got_lanman,
			 char *md4,  int *got_md4,
			 DWORD rid
			 )
{
	des_key_schedule ks1, ks2;
	des_cblock deskey1, deskey2;
	int username_offset = get_int(vp + 0xC);
	int username_len = get_int(vp + 0x10); 
	int fullname_offset = get_int(vp + 0x18);
	int fullname_len = get_int(vp + 0x1c);
	int comment_offset = get_int(vp + 0x24);
	int comment_len = get_int(vp + 0x28);
	int homedir_offset = get_int(vp + 0x48);
	int homedir_len = get_int(vp + 0x4c);
	int pw_offset = get_int(vp + 0x9c);

	*username = 0;
	*fullname = 0;
	*comment = 0;
	*homedir = 0;
	*got_lanman = 0;
	*got_md4 = 0;

	if(username_len < 0 || username_offset < 0 || comment_len < 0 ||
			   fullname_len < 0 || homedir_offset < 0 ||
		       comment_offset < 0 || pw_offset < 0)
		return -1;
	username_offset += 0xCC;
	fullname_offset += 0xCC;
	comment_offset += 0xCC;
	homedir_offset += 0xCC;
	pw_offset += 0xCC;

	if((*username = (char *)malloc(username_len + 1)) == 0) {
		fprintf(stderr, "check_vp: malloc fail for username.\n");
		return -1;
	}
	if((*fullname = (char *)malloc(fullname_len + 1)) == 0) {
		fprintf(stderr, "check_vp: malloc fail for username.\n");
		free(*username);
		*username = 0;
		return -1;
	}
	if((*comment = (char *)malloc(comment_len + 1)) == 0) {
		fprintf(stderr, "check_vp: malloc fail for comment.\n");
		free(*username);
		*username = 0;
		free(*fullname);
		*fullname = 0;
		return -1;
	}
	if((*homedir = (char *)malloc(homedir_len + 1)) == 0) {
		fprintf(stderr, "check_vp: malloc fail for homedir.\n");
		free(*username);
		*username = 0;
		free(*fullname);
		*fullname = 0;
		free(*comment);
		*comment = 0;
		return -1;
	}
	wcstombs( *username, (wchar_t *)(vp + username_offset), username_len/sizeof(wchar_t));
	(*username)[username_len/sizeof(wchar_t)] = 0;
	wcstombs( *fullname, (wchar_t *)(vp + fullname_offset), fullname_len/sizeof(wchar_t));
	(*fullname)[fullname_len/sizeof(wchar_t)] = 0;
	wcstombs( *comment, (wchar_t *)(vp + comment_offset), comment_len/sizeof(wchar_t));
	(*comment)[comment_len/sizeof(wchar_t)] = 0;
	wcstombs( *homedir, (wchar_t *)(vp + homedir_offset), homedir_len/sizeof(wchar_t));
	(*homedir)[homedir_len/sizeof(wchar_t)] = 0;

	if(pw_offset >= vp_size) {
		/* No password */
		*got_lanman = 0;
		*got_md4 = 0;
		return 0;
	}

	/* Check that the password offset plus the size of the
	   lanman and md4 hashes fits within the V record. */
	if(pw_offset + 32 > vp_size) {
		/* Account disabled ? */
		*got_lanman = -1;
		*got_md4 = -1;
		return 0;
	}

	/* Get the two decrpt keys. */
	sid_to_key1(rid,(unsigned char *)deskey1);
	des_set_key((des_cblock *)deskey1,ks1);
	sid_to_key2(rid,(unsigned char *)deskey2);
	des_set_key((des_cblock *)deskey2,ks2);
	
	vp += pw_offset;
	/* Decrypt the lanman password hash as two 8 byte blocks. */
	des_ecb_encrypt((des_cblock *)vp,
					(des_cblock *)lanman, ks1, DES_DECRYPT);
	des_ecb_encrypt((des_cblock *)(vp + 8),
					(des_cblock *)&lanman[8], ks2, DES_DECRYPT);

	vp += 16;
	/* Decrypt the NT md4 password hash as two 8 byte blocks. */
	des_ecb_encrypt((des_cblock *)vp,
					(des_cblock *)md4, ks1, DES_DECRYPT);
	des_ecb_encrypt((des_cblock *)(vp + 8),
					(des_cblock *)&md4[8], ks2, DES_DECRYPT);

	*got_lanman = 1;
	*got_md4 = 1;
	return 0;
}

/*
 * Function to print out a 16 byte array as hex.
 */

void print_hexval(char *val)
{
	int i;
	for(i = 0; i < 16; i++)
		printf("%02X", (unsigned char)val[i]);
}

/* 
 * Function to strip out any ':' or '\n', '\r' from a text
 * string.
 */

void strip_text( char *txt )
{
	char *p;
	for( p = strchr(txt, ':'); p ; p = strchr( p + 1, ':'))
		*p = '_';
	for( p = strchr(txt, '\n'); p ; p = strchr(p + 1, '\n'))
		*p = '_';										   
	for( p = strchr(txt, '\r'); p ; p = strchr(p + 1, '\r'))
		*p = '_';
}

/*
 * Function to dump a users smbpasswd entry onto stdout.
 * Returns 0 on success, -1 on fail.
 */

int printout_smb_entry( HKEY user, DWORD rid )
{
 	DWORD err;
	DWORD type;
	DWORD size = 0;
	char *vp;
	char lanman[16];
	char md4_hash[16];
	char *username;
	char *fullname;
	char *comment;
	char *homedir;
	int got_lanman;
	int got_md4;

	/* Find out how much space we need for the 'V' value. */
	if((err = RegQueryValueEx( user, "V", 0, &type, 0, &size)) 
								!= ERROR_SUCCESS) {
		fprintf(stderr, "printout_smb_entry: Unable to determine size needed \
for user 'V' value. Error was %s.\n.", str_oserr(err));
		return -1;
	}
	if((vp = (char *)malloc(size)) == 0) {
		fprintf(stderr, "printout_smb_entry: malloc fail for user entry.\n");
		return -1;
	}
	if((err = RegQueryValueEx( user, "V", 0, &type, (LPBYTE)vp, &size)) 
								!= ERROR_SUCCESS) {
		fprintf(stderr, "printout_smb_entry: Unable to read user 'V' value. \
Error was %s.\n.", str_oserr(err));
		free(vp);
		return -1;
	}
	/* Check heuristics */
	if(check_vp(vp, size, &username, &fullname, &comment, 
						&homedir, lanman, &got_lanman, 
		               md4_hash, &got_md4, rid) != 0) {
		fprintf(stderr, "Failed to parse entry for RID %X\n", rid);
		free(vp);
		return 0;
	}
	/* Ensure username of comment don't have any nasty suprises
	   for us such as an embedded ':' or '\n' - see multiple UNIX
	   passwd field update security bugs for details... */
	strip_text( username );
	strip_text( fullname );
	strip_text( comment );
	/* If homedir contains a drive letter this mangles it - but it protects
	   the integrity of the smbpasswd file. */
	strip_text( homedir );

	printf("%s:%d:", username, rid);
	if(got_lanman) {
		if(got_lanman == -1) /* Disabled account ? */
			printf("********************************");
		else
			print_hexval(lanman);
	} else
		printf("NO PASSWORD*********************");
	printf(":");
	if(got_md4) {
		if(got_md4 == -1)  /* Disabled account ? */
			printf("********************************");
		else
			print_hexval(md4_hash);
	} else
		printf("NO PASSWORD*********************");
	printf(":");
	if(*fullname)
		printf("%s", fullname);
	if(*fullname && *comment)
		printf(",");
	if(*comment)
		printf("%s", comment);
	printf(":");
	if(*homedir)					   
		printf("%s", homedir);
	printf(":\n");

	free(username);
	free(comment);
	free(homedir);
	free(vp);
	return 0;
}

/*
 * Function to go through all the user SID's - dumping out
 * their SAM values. Returns 0 on success, -1 on fail.
 */

int enumerate_users( HKEY key)
{
	DWORD indx = 0;
	DWORD err;
	DWORD rid;
	char usersid[128];

	do {
		DWORD size;
		FILETIME ft;

		size = sizeof(usersid);
		err = RegEnumKeyEx(	key, indx, usersid, &size, 0, 0, 0, &ft);
		if(err == ERROR_SUCCESS) {
			HKEY subkey;

			indx++;
			if((err = RegOpenKeyEx( key, usersid, 0, KEY_QUERY_VALUE, &subkey)) !=
						ERROR_SUCCESS) {
				fprintf(stderr, "enumerate_users: Failed to open key %s to read value. \
Error was %s.\n",
						usersid, str_oserr(err));
				RegCloseKey(key);
				return -1;
			}
			rid = strtoul(usersid, 0, 16);
			/* Hack as we know there is a Names key here */
			if(rid != 0) {
				if(printout_smb_entry( subkey, rid ) != 0) {
					RegCloseKey(subkey);
					return -1;
				}
			}
			RegCloseKey(subkey);
		}
	} while(err == ERROR_SUCCESS);

	if(err != ERROR_NO_MORE_ITEMS) {
		RegCloseKey(key);
		return -1;
	}
	return 0;
}

/*
 * Print usage message and die.
 */
void usage(const char *arg0) {
	fprintf(stderr, "Usage: %s <\\\\machine>\n", arg0);
	exit(-1);
}

/*
 * usage: \\machine
 */

int main(int argc, char **argv)
{
	char username[128];
	DWORD size;
	HKEY start_key = HKEY_LOCAL_MACHINE;
	HKEY users_key;
	int err;

	if(argc > 2)
		usage(argv[0]);

	/*
	 * Ensure we are running as Administrator before
	 * we will run.
	 */
	size = sizeof(username);
	if(GetUserName(username, &size)== FALSE) {
		fprintf(stderr, "%s: GetUserName() failed. Error was %s.", 
			argv[0], str_oserr(GetLastError()));
		return -1;
	}

	if(stricmp( "Administrator", username) != 0) {
		fprintf(stderr, "%s: You must be running as user Administrator \
to run this program\n", argv[0]);
		return -1;
	}

	/* 
	 * Open a connection to the remote machines registry.
	 */
	if(argc == 2) {
		if((err = RegConnectRegistry( argv[1], HKEY_LOCAL_MACHINE, &start_key)) !=
			ERROR_SUCCESS) {
			fprintf(stderr, "%s: Failed to connect to registry on remote computer %s.\
Error was %s.\n", argv[0], argv[1], str_oserr(err));
			return -1;
		}
	}

	/* 
	 * We need to get to HKEY_LOCAL_MACHINE\SECURITY\SAM\Domains\Account\Users.
	 * The security on this key normally doesn't allow Administrators
	 * to read - we need to add this.
	 */

	if((err = set_sam_tree_access( start_key, &users_key)) != 0) {
		if(err == -2)
			restore_sam_tree_access( start_key);
		return -1;
	}
	/* Print the users SAM entries in smbpasswd format onto stdout. */
	enumerate_users( users_key );
	RegCloseKey(users_key);
	/* reset the security on the SAM */
	restore_sam_tree_access( start_key );
	if(start_key != HKEY_LOCAL_MACHINE)
		RegCloseKey(start_key);
	return 0;
}