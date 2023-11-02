// export R# package module type define for javascript/typescript language
//
//    imports "GNUplot" from "GNUplot";
//
// ref=GNUplot.Rscript@GNUplot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GNUplot {
   /**
   */
   function config(gnuplot: string): boolean;
   /**
     * @param env default value Is ``null``.
   */
   function scatter(x: any, y: any, env?: object): any;
}
