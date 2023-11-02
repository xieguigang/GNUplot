// export R# package module type define for javascript/typescript language
//
//    imports "GNUplot" from "GNUplot";
//
// ref=GNUplot.Rscript@GNUplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace GNUplot {
   /**
    * config of the gnuplot executable file path
    * 
    * 
     * @param gnuplot the user configed custom gnuplot path location.
   */
   function config(gnuplot: string): boolean;
   /**
    * Create x,y scatter plot
    * 
    * 
     * @param x -
     * @param y -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function scatter(x: any, y: any, env?: object): any;
}
