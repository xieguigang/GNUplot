require(gnuplot);

setwd(@dir);

let x = runif(100, -30, 30);
let y = runif(100, -50, 50);
let z = runif(100, -20, 20);

GNUplot::pointCloud(x,y,z, file = "./pointCloud.png");