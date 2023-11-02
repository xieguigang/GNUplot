
#' Get the executable file path of the gnuplot program
#' 
#' @details the function returns different file path location 
#'    between the windows platform and the centos platform,
#' 
#' 1. for windows, this function always returns the package built-in 
#'      re-distribution of gnuplot program
#' 2. for centos, this function always returns the file path location: 
#'      ``/usr/bin/gnuplot``, as this file path location is the default 
#'    install location of the gnuplot on centos with 
#'      ``yum install gnuplot`` command.
#' 
const __get_gnuplot_ = function() {
    const win32_path = system.file("gnuplot/bin/gnuplot.exe", package = "gnuplot");
    const unix_path  = "/usr/bin/gnuplot";
    const sys        = Sys.info()$sysname;

    if (sys == "Unix") {
        unix_path; 
    } else {
        win32_path;
    }
}