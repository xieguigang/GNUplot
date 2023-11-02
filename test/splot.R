require(gnuplot);

setwd(@dir);

GNUplot::splot("1 / (0.05*x*x + 0.05*y*y + 1)", file = "./splot.png");