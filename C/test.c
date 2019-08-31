#include <stdio.h>

int sayhi(char *s)
{
	printf("say:%s\n",s);
	return 1;
}
int main(int argc, char *argv[])
{
	sayhi(argv[0]);
	return 0;
	
}
