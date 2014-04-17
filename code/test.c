#include "basket.h"

int main(int argc, char **argv)
{
	if (argc != 2) {
		printf("Usage: %s basket_id\nFor example: %s 1000\n", argv[0], argv[0]);
		return 0;
	}

	char filename[1024];

	sprintf(filename, "../data/basket_%06d.dat", atoi(argv[1]));

        FILE *fp = fopen(filename, "rb");

	if (!fp) {
		printf("Cannot open %s\n", filename);
		return 0;
	}

        basket_t *basket = read_basket(fp);

	print_basket(basket);

        fclose(fp);

        free_basket(basket);
	
	return 0;
}

