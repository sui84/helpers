#ifndef __MYSOCK_H
#define __MYSOCK_H

#ifdef __cplusplus
extern "C" {
#endif

	//linux�����ý��̽���daemon״̬��windows��Ч
	void set_daemon();

	//windows�µ��õĳ�ʼ��socket������linux����Ҫ
	void init_socket();

	//windows�µ��õ��ͷ�socket������linux����Ҫ
	void free_socket();

	//������ת��ΪIP��domain��������������ֵΪstruct in_addr��s_addr��Ա
	unsigned int domain2ip(const char *domain);

	//����һ��socket��
	//status = 1 is TCP, other is UDP������ֵ>=0����ɹ�������socket�����-1ʧ��
	int create_socket(int status);

	//�ر���create_socket������socket,
	//sockΪcreate_socket�������ص�socket���
	void close_socket(int sock);

	//Ϊsocket��һ���˿ں�
	//sockΪcreate_socket�������ص�socket�����portΪҪ�󶨵Ķ˿ںš�����ֵ=0�ɹ���-1ʧ��
	int bind_socket(int sock, unsigned short port);

	//ʹ��UDPЭ���������
	//sockΪcreate_socket�������ص�socket�����bufΪ�������ݻ������� bufsizeΪ��������С����λ���ֽڣ�, srcIPΪ������ԴIP��ַ������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int udp_recv(int sock, char *buf, size_t bufsize, char *srcIP);

	//ʹ��UDPЭ�鷢������
	//sockΪcreate_socket�������ص�socket�����bufΪ�������ݻ������� bufsizeΪ�������ݴ�С����λ���ֽڣ���destIPΪ����Ŀ��IP��ַ��portΪ����Ŀ�Ķ˿ں� ������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int udp_send(int sock, const char *destIP, unsigned short port, const char *buf, size_t bufsize);

	//ʹ��TCPЭ���������֮ǰ��Ҫ��listen
	//sockΪcreate_socket�������ص�socket���������ֵ=0�ɹ�,-1ʧ��
	int tcp_listen(int sock);

	//ʹ��TCPЭ��������Կͻ��˵�����
	//sockΪcreate_socket�������ص�socket����� srcIPΪ���Կͻ��˵�IP��ַ������ֵ>0�������Կͻ��˵�socket���,-1ʧ��
	int tcp_accept(int sock, char *srcIP);

	//ʹ��TCPЭ�����ӵ�ָ���ķ�����
	//sockΪcreate_socket�������ص�socket�����destIPΪ���Է���˶˵�IP��ַ��portΪ����˵Ķ˿ںš�����ֵ=0�ɹ�,-1ʧ��
	int tcp_connect(int sock, const char *destIP, unsigned short port);

	//ʹ��TCPЭ���������
	//sockΪcreate_socket�������ص�socket�����bufΪ�������ݻ������� bufsizeΪ��������С����λ���ֽڣ�������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int tcp_recv(int sock, char *buf, size_t bufsize);

	//ʹ��TCPЭ�鷢������
	//sockΪcreate_socket�������ص�socket�����bufΪ�������ݻ������� bufsizeΪ�������ݴ�СŶ����λ���ֽڣ�������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int tcp_send(int sock, const char *buf, size_t bufsize);

	////////////////////////////////����Ϊapp����//////////////////////////

	//ʹ��UDPЭ���������
	//bufΪ�������ݻ������� bufsizeΪ��������С����λ���ֽڣ�, portΪ�������ݵĶ˿ںţ�srcIPΪ������ԴIP��ַ������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int app_udp_recv(char *buf, size_t bufsize, unsigned short port, char *srcIP);

	//ʹ��UDPЭ�鷢������
	//bufΪ�������ݻ������� bufsizeΪ�������ݴ�С����λ���ֽڣ���destIPΪ����Ŀ��IP��ַ��portΪ����Ŀ�Ķ˿ں� ������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int app_udp_send(const char *destIP, unsigned short port, const char *buf, size_t bufsize);

	//ʹ��TCPЭ��ͻ������ӵ�ָ���ķ����
	//destIPΪ���Է���˵�IP��ַ��portΪ����˵Ķ˿ںš�����ֵ=0�ɹ�,-1ʧ��
	int app_client_connect(const char *destIP, unsigned short port);

	//�Ͽ��ͻ���TCP����
	void app_client_close();

	//�Ͽ������TCP����
	void app_server_close();

	//ʹ��TCPЭ��ͻ��˽�������
	//bufΪ�������ݻ�������bufsizeΪ��������С����λ���ֽڣ�������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int app_client_recv(char *buf, size_t bufsize);

	//ʹ��TCPЭ��ͻ��˷�������
	//bufΪ�������ݻ������� bufsizeΪ�������ݴ�СŶ����λ���ֽڣ�������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int app_client_send(const char *buf, size_t bufsize);


	//ʹ��TCPЭ�飬���������socket
	//����portΪ����˶˿ںţ�����ֵ=0�ɹ�,-1ʧ��
	int app_server_create(unsigned short port);

	//ʹ��TCPЭ�����˽�������
	//bufΪ�������ݻ�������bufsizeΪ��������С����λ���ֽڣ�,srcIPΪ�ͻ���IP��ַ������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int app_server_recv(char *buf, size_t bufsize, char *srcIP);

	//ʹ��TCPЭ�����˷�������
	//bufΪ�������ݻ������� bufsizeΪ�������ݴ�СŶ����λ���ֽڣ�������ֵ>0�ɹ����������ֽ�����=0���������رգ�-1ʧ��
	int app_server_send(const char *buf, size_t bufsize);

#ifdef __cplusplus
}
#endif

#endif