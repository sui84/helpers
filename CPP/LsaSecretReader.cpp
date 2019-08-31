// LsaSecretReader.cpp : Defines the entry point for the console application.
#include "stdafx.h"
#include <windows.h>
#include <stdio.h>
#include <ntsecapi.h>


#pragma comment (lib, "Advapi32")

PLSA_UNICODE_STRING InitLsaString(IN LPWSTR wszString, OUT PLSA_UNICODE_STRING lsastr)
{
	if ( !lsastr )
		return NULL;

	if ( wszString )
	{
		lsastr->Buffer=wszString;
		lsastr->Length=(USHORT)lstrlenW(wszString)*sizeof(WCHAR);
		lsastr->MaximumLength=lsastr->Length+2;
	}
	else
	{
		lsastr->Buffer=L"";
		lsastr->Length=0;
		lsastr->MaximumLength=2;
	}

	return lsastr;
}


int _tmain(int argc, _TCHAR* argv[])
{
	NTSTATUS status;
	LSA_OBJECT_ATTRIBUTES att;
	LSA_HANDLE pol;
	LSA_UNICODE_STRING secret, *data=NULL;

	if ( argc!=2 )
	{
		_tprintf(TEXT("Syntax: %s secretnamen"),argv[0]);
		return 1;
	}
	
	memset(&att,0,sizeof(att));
	
	status=LsaOpenPolicy(NULL,&att,0,&pol);
	if ( status!=ERROR_SUCCESS )
	{
		_tprintf(TEXT("LsaOpenPolicy error: %lXn"),status);
		return 2;
	}

	InitLsaString(argv[1],&secret);
	status=LsaRetrievePrivateData(pol,&secret,&data);
	if ( status!=ERROR_SUCCESS )
	{
		_tprintf(TEXT("LsaRetrievePrivateData error: %lXn"),status);
		return 3;
	}
	LsaClose(pol);

	if ( data && data->Buffer && data->Length )
	{
		for ( USHORT i=0; i<data->Length; i+=16 )
		{
			_tprintf(TEXT("%04X: "),i);
			LPBYTE ptr=(LPBYTE)data->Buffer;
			ptr+=i;
			for ( int j=0; j<16; j++ )
				_tprintf(TEXT("%02X "),ptr[j]);
			_tprintf(TEXT("n"));
		}
	}
	else
	{
		_tprintf(TEXT("No datan"));
	}

	return 0;
}

int main (int argc, char **argv)
    {
        int l_var = 1;
        printf("hello world!");
        printf("g_var = %d, l_var = %d.\r\n", l_var, l_var);
        return 0;
    }
