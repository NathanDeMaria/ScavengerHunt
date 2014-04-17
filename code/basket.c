#include "basket.h"

/**
 * print an item
 */
void print_item(item_t *item)
{
    /* item cannot be NULL */
    if (item == NULL) {
        fprintf(stderr, "The item is NULL\n");
        exit(0);
    }

    /* print item id and review */
    printf("Item: %d\n",    item->item_id);
    printf("Review: %s\n",  item->review);
}


/**
 * print a basket
 */
void print_basket(basket_t *basket)
{
    int i;

    /* basket cannnot be NULL */
    if (basket == NULL) {
        fprintf(stderr, "The basket is NULL\n");
        exit(0);
    }

    /* print customer's id */
    printf("ID: %d\n", basket->customer_id);

    /* print customer's state */
    printf("State: %s\n", basket->state);

    /* print weekday of purchase */
    printf("Date: %s\n", basket->weekday);

    /* print customer's basket */
    for (i = 0; i < basket->item_num; i++) {
        print_item(&(basket->items[i]));
    }
}

/**
 * read an item from a file
 */
void read_item(item_t *item, FILE *fp)
{
    /* Assume file has been opened */
    if (fp == NULL) {
        fprintf(stderr, "The file stream is NULL\n");
        exit(0);
    }

    /* item cannot be NULL */
    if (item == NULL) {
        fprintf(stderr, "The message is NULL\n");
        exit(0);
    }

    /* read the item */
    fread(&(item->item_id),     sizeof(int),    1,          fp);
    fread(&(item->review[0]),   sizeof(char),   TEXT_LONG,  fp);
}


/**
 * read a basket from a file
 */
basket_t *read_basket(FILE *fp)
{
    int i;

    /* Assume file has been opened */
    if (fp == NULL) {
        fprintf(stderr, "The file stream is NULL\n");
        exit(0);
    }

    /* allocate memory for the basket */
    basket_t *basket = (basket_t *)malloc(sizeof(basket_t));

    fread(&(basket->customer_id),    sizeof(int),    1,          fp);
    fread(&(basket->state[0]),       sizeof(char),   TEXT_SHORT, fp);
    fread(&(basket->weekday[0]),     sizeof(char),   TEXT_SHORT, fp);
    fread(&(basket->item_num),       sizeof(int),    1,          fp);

    /* initialize items */
    basket->items = NULL;

    /* allocate memory for items */
    if (basket->item_num > 0) {

        basket->items = (item_t *)malloc(sizeof(item_t) * basket->item_num);

        for(i = 0; i < basket->item_num; i++) {
            read_item(&(basket->items[i]), fp);
        }
    }

    /* return the basket */
    return basket;
}


/**
 * free memory of a basket
 */
void free_basket(basket_t *basket)
{
    if (basket == NULL) {
        return;
    }

    /* free item memory */
    if (basket->items != NULL) {
        free(basket->items);
    }

    /* free basket memory */
    free(basket);
}

