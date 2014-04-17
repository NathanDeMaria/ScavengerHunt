#ifndef __BASKET_H__
#define __BASKET_H__

#include <stdio.h>
#include <stdlib.h>

#define TEXT_SHORT      64
#define TEXT_LONG       1024

/* item structure */
typedef struct {
    int     item_id;
    char    review[TEXT_LONG];
} item_t;

/* basket structure */
typedef struct {
    int     customer_id;
    char    state[TEXT_SHORT];
    char    weekday[TEXT_SHORT];
    int     item_num;
    item_t *items;
} basket_t;

/**
 * print a basket
 */
void print_basket(basket_t *basket);

/**
 * read a basket from a file
 */
basket_t *read_basket(FILE *fp);

/**
 * free memory of a basket
 */
void free_basket(basket_t *basket);

#endif
