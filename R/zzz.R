imports "gnuplot" from "GNUplot";

const .onLoad = function() {
    gnuplot::config(__get_gnuplot_());
}

