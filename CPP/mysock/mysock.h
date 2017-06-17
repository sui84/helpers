#ifndef __MYSOCK_H
#define __MYSOCK_H

#ifdef __cplusplus
extern "C" {
#endif

	//linux下设置进程进入daemon状态，windows无效
	void set_daemon();

	//windows下调用的初始化socket环境，linux不需要
	void init_socket();

	//windows下调用的释放socket环境，linux不需要
	void free_socket();

	//将域名转化为IP，domain代表域名，返回值为struct in_addr的s_addr成员
	unsigned int domain2ip(const char *domain);

	//建立一个socket，
	//status = 1 is TCP, other is UDP。返回值>=0代表成功建立的socket句柄，-1失败
	int create_socket(int status);

	//关闭由create_socket创建的socket,
	//sock为create_socket函数返回的socket句柄
	void close_socket(int sock);

	//为socket绑定一个端口号
	//sock为create_socket函数返回的socket句柄，port为要绑定的端口号。返回值=0成功，-1失败
	int bind_socket(int sock, unsigned short port);

	//使用UDP协议接收数据
	//sock为create_socket函数返回的socket句柄，buf为接收数据缓冲区， bufsize为缓冲区大小（单位：字节）, srcIP为数据来源IP地址。返回值>0成功接收数据字节数，=0连接正常关闭，-1失败
	int udp_recv(int sock, char *buf, size_t bufsize, char *srcIP);

	//使用UDP协议发送数据
	//sock为create_socket函数返回的socket句柄，buf为发送数据缓冲区， bufsize为发送数据大小（单位：字节），destIP为发送目的IP地址，port为发送目的端口号 。返回值>0成功发送数据字节数，=0连接正常关闭，-1失败
	int udp_send(int sock, const char *destIP, unsigned short port, const char *buf, size_t bufsize);

	//使用TCP协议接收数据之前需要先listen
	//sock为create_socket函数返回的socket句柄。返回值=0成功,-1失败
	int tcp_listen(int sock);

	//使用TCP协议接收来自客户端的链接
	//sock为create_socket函数返回的socket句柄， srcIP为来自客户端的IP地址。返回值>0代表来自客户端的socket句柄,-1失败
	int tcp_accept(int sock, char *srcIP);

	//使用TCP协议连接到指定的服务器
	//sock为create_socket函数返回的socket句柄，destIP为来自服务端端的IP地址，port为服务端的端口号。返回值=0成功,-1失败
	int tcp_connect(int sock, const char *destIP, unsigned short port);

	//使用TCP协议接收数据
	//sock为create_socket函数返回的socket句柄，buf为接收数据缓冲区， bufsize为缓冲区大小（单位：字节）。返回值>0成功接收数据字节数，=0连接正常关闭，-1失败
	int tcp_recv(int sock, char *buf, size_t bufsize);

	//使用TCP协议发送数据
	//sock为create_socket函数返回的socket句柄，buf为发送数据缓冲区， bufsize为发送数据大小哦（单位：字节）。返回值>0成功发送数据字节数，=0连接正常关闭，-1失败
	int tcp_send(int sock, const char *buf, size_t bufsize);

	////////////////////////////////以下为app函数//////////////////////////

	//使用UDP协议接收数据
	//buf为接收数据缓冲区， bufsize为缓冲区大小（单位：字节）, port为接收数据的端口号，srcIP为数据来源IP地址。返回值>0成功接收数据字节数，=0连接正常关闭，-1失败
	int app_udp_recv(char *buf, size_t bufsize, unsigned short port, char *srcIP);

	//使用UDP协议发送数据
	//buf为发送数据缓冲区， bufsize为发送数据大小（单位：字节），destIP为发送目的IP地址，port为发送目的端口号 。返回值>0成功发送数据字节数，=0连接正常关闭，-1失败
	int app_udp_send(const char *destIP, unsigned short port, const char *buf, size_t bufsize);

	//使用TCP协议客户端连接到指定的服务端
	//destIP为来自服务端的IP地址，port为服务端的端口号。返回值=0成功,-1失败
	int app_client_connect(const char *destIP, unsigned short port);

	//断开客户端TCP连接
	void app_client_close();

	//断开服务端TCP连接
	void app_server_close();

	//使用TCP协议客户端接收数据
	//buf为接收数据缓冲区，bufsize为缓冲区大小（单位：字节）。返回值>0成功接收数据字节数，=0连接正常关闭，-1失败
	int app_client_recv(char *buf, size_t bufsize);

	//使用TCP协议客户端发送数据
	//buf为发送数据缓冲区， bufsize为发送数据大小哦（单位：字节）。返回值>0成功发送数据字节数，=0连接正常关闭，-1失败
	int app_client_send(const char *buf, size_t bufsize);


	//使用TCP协议，建立服务端socket
	//参数port为服务端端口号，返回值=0成功,-1失败
	int app_server_create(unsigned short port);

	//使用TCP协议服务端接收数据
	//buf为接收数据缓冲区，bufsize为缓冲区大小（单位：字节）,srcIP为客户端IP地址。返回值>0成功接收数据字节数，=0连接正常关闭，-1失败
	int app_server_recv(char *buf, size_t bufsize, char *srcIP);

	//使用TCP协议服务端发送数据
	//buf为发送数据缓冲区， bufsize为发送数据大小哦（单位：字节）。返回值>0成功发送数据字节数，=0连接正常关闭，-1失败
	int app_server_send(const char *buf, size_t bufsize);

#ifdef __cplusplus
}
#endif

#endif