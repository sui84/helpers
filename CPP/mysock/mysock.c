#include <string.h>
#include <stdlib.h>
#include <stdio.h>

#ifndef __UNIX
#include <WinSock2.h>
#pragma comment(lib, "Ws2_32.lib")
#pragma warning(disable:4996)
#else
#include <sys/types.h>
#include <sys/socket.h>
#include <unistd.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <netdb.h>
#endif

//如果编译动态库，需要把#define STATIC_LIB注释
#define STATIC_LIB

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
void set_daemon()
{
#ifdef __UNIX
	pid_t pid, sid;
	pid = fork();
	if (pid < 0)
	{
		exit(-1);
	}
	if (pid > 0)
	{
		exit(0);
	}

	if ((sid = setsid()) < 0)
	{
		exit(-1);
	}
#endif
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
void init_socket()
{
#ifndef __UNIX
	unsigned short ver;
	WSADATA wsaData;
	ver = MAKEWORD(1, 1);
	WSAStartup(ver, &wsaData);
#endif
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
void free_socket()
{
#ifndef __UNIX
	WSACleanup();
#endif
}


#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
unsigned int domain2ip(const char *domain)
{
	struct hostent *iphost = gethostbyname(domain);
	if (iphost)
	{
		struct in_addr *p = (struct in_addr *)iphost->h_addr_list[0];
		if (p)
			return p->s_addr;
	}
	return 0;
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int create_socket(int status)
{
#ifndef __UNIX
	char on = 1;
#else
	int on = 1;
#endif
	int st = 0;
	if (status == 1)
	{
		st = socket(AF_INET, SOCK_STREAM, 0);//建立TCP socket
		setsockopt(st, SOL_SOCKET, SO_REUSEADDR, &on, sizeof(on));//设置socket地址可重用
	}
	else
	{
		st = socket(AF_INET, SOCK_DGRAM, 0);//建立UDP socket
		setsockopt(st, SOL_SOCKET, SO_BROADCAST, &on, sizeof(on));//设置可以发送udp广播消息
	}
	return st;
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
void close_socket(int sock)
{
#ifndef __UNIX
	closesocket(sock);
#else
	close(sock);
#endif
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int bind_socket(int sock, unsigned short port)
{
	struct sockaddr_in recv_addr;
	memset(&recv_addr, 0, sizeof(recv_addr));
	recv_addr.sin_family = AF_INET;
	recv_addr.sin_port = htons(port);//指定port为要连接的端口号
	recv_addr.sin_addr.s_addr = htonl(INADDR_ANY);//指定接收端IP地址为任意
	return bind(sock, (struct sockaddr *) &recv_addr, sizeof(recv_addr));
}


#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int udp_recv(int sock, char *buf, size_t bufsize, char *srcIP)
{
	struct sockaddr_in addr;
	memset(&addr, 0, sizeof(addr));
#ifndef __UNIX
	int len = sizeof(addr);
#else
	socklen_t len = sizeof(addr);
#endif
	unsigned int rc = recvfrom(sock, buf, bufsize, 0, (struct sockaddr *) &addr, &len);//接收到来自srcIP的消息
	//strcpy(srcIP, inet_ntoa(addr.sin_addr));
	unsigned char *c = (unsigned char *)&addr.sin_addr.s_addr;
	sprintf(srcIP, "%u.%u.%u.%u", c[0], c[1], c[2], c[3]);
	return rc;
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int udp_send(int sock, const char *destIP, unsigned short port, const char *buf, size_t bufsize)
{
	struct sockaddr_in addr;
	memset(&addr, 0, sizeof(addr));
	addr.sin_family = AF_INET;
	addr.sin_port = htons(port);//指定port为要连接的端口号
	//addr.sin_addr.s_addr = inet_addr(destIP);//指定IP为要连接的IP地址	
	addr.sin_addr.s_addr = domain2ip(destIP);
	return sendto(sock, buf, bufsize, 0, (struct sockaddr *) &addr, sizeof(addr));//向指定IP发送消息
}


#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int tcp_listen(int sock)
{
	return listen(sock, 20);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int tcp_accept(int sock, char *srcIP)
{
	struct sockaddr_in addr;
	memset(&addr, 0, sizeof(addr));
#ifndef __UNIX
	int len = sizeof(addr);
#else
	socklen_t len = sizeof(addr);
#endif
	int st = accept(sock, (struct sockaddr *) &addr, &len);
	//strcpy(srcIP, inet_ntoa(addr.sin_addr));
	unsigned char *c = (unsigned char *)&addr.sin_addr.s_addr;
	sprintf(srcIP, "%u.%u.%u.%u", c[0], c[1], c[2], c[3]);
	return st;

}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int tcp_connect(int sock, const char *destIP, unsigned short port)
{
	struct sockaddr_in addr;
	memset(&addr, 0, sizeof(addr));
	addr.sin_family = AF_INET;
	addr.sin_port = htons(port);//指定port为要连接的端口号
	//addr.sin_addr.s_addr = inet_addr(destIP);//指定IP为要连接的IP地址	
	addr.sin_addr.s_addr = domain2ip(destIP);
	return connect(sock, (struct sockaddr *) &addr, sizeof(addr));//连接到指定IP
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int tcp_recv(int sock, char *buf, size_t bufsize)
{
	return recv(sock, buf, bufsize, 0);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int tcp_send(int sock, const char *buf, size_t bufsize)
{
	return send(sock, buf, bufsize, 0);
}

////////////////////////////////以下为app函数//////////////////////////

static int is_init = 0;
static int udp_recv_sock = -1;
static int udp_send_sock = -1;
static int tcp_client_sock = -1;
static int tcp_server_sock = -1;
static int tcp_list_sock = -1;

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_udp_recv(char *buf, size_t bufsize, unsigned short port, char *srcIP)
{
	if (is_init == 0)
	{
		init_socket();
		is_init = 1;
	}
	if (udp_recv_sock != -1)
		close_socket(udp_recv_sock);

	udp_recv_sock = create_socket(0);
	int rc = bind_socket(udp_recv_sock, port);
	if (rc < 0)
		return rc;

	return udp_recv(udp_recv_sock, buf, bufsize, srcIP);
}


#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_udp_send(const char *destIP, unsigned short port, const char *buf, size_t bufsize)
{
	if (is_init == 0)
	{
		init_socket();
		is_init = 1;
	}
	if (udp_send_sock == -1)
	{
		udp_send_sock = create_socket(0);
	}

	return udp_send(udp_send_sock, destIP, port, buf, bufsize);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_client_connect(const char *destIP, unsigned short port)
{
	if (is_init == 0)
	{
		init_socket();
		is_init = 1;
	}

	if (tcp_client_sock != -1)
		close_socket(tcp_client_sock);

	tcp_client_sock = create_socket(1);
	return tcp_connect(tcp_client_sock, destIP, port);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
void app_client_close()
{
	if (tcp_client_sock != -1)
	{
		close_socket(tcp_client_sock);
		tcp_client_sock = -1;
	}
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
void app_server_close()
{
	if (tcp_server_sock != -1)
	{
		close_socket(tcp_server_sock);
		tcp_server_sock = -1;
	}
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_client_recv(char *buf, size_t bufsize)
{
	return tcp_recv(tcp_client_sock, buf, bufsize);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_client_send(const char *buf, size_t bufsize)
{
	return tcp_send(tcp_client_sock, buf, bufsize);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_server_create(unsigned short port)
{
	if (is_init == 0)
	{
		init_socket();
		is_init = 1;
	}
	if (tcp_list_sock != -1)
	{
		close_socket(tcp_list_sock);
	}

	tcp_list_sock = create_socket(1);
	int rc = bind_socket(tcp_list_sock, port);
	if (rc == -1)
		return rc;

	return tcp_listen(tcp_list_sock);
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_server_recv(char *buf, size_t bufsize, char *srcIP)
{
	static char IP[100];
	if (tcp_server_sock <= 0)
	{
		memset(IP, 0, sizeof(IP));
		tcp_server_sock = tcp_accept(tcp_list_sock, IP);
	}
	strcpy(srcIP, IP);
	int rc = tcp_recv(tcp_server_sock, buf, bufsize);
	if (rc <= 0)
		app_server_close();
	return rc;
}

#ifndef __UNIX
#ifndef STATIC_LIB
__declspec(dllexport)
#endif
#endif
int app_server_send(const char *buf, size_t bufsize)
{
	return tcp_send(tcp_server_sock, buf, bufsize);
}


