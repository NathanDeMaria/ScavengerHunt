#!/bin/sh
for i in {0..2999}
do
	./a.out $i > `printf "modified_basket_%06i.dat\n" $i`
done
