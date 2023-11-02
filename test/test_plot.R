require(gnuplot);

setwd(@dir);


let x = [-10, -8.5, -2, 1, 6, 9, 10, 14, 15, 19];
let y = [-4, 6.5, -2, 3, -8, -5, 11, 4, -5, 10];

GNUplot::scatter(x,y);