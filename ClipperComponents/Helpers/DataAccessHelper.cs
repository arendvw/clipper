using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StudioAvw.Clipper.Components.Helpers {
  static class DataAccessHelper {


    /// <summary>
    /// Fetch data at index position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="da"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static T Fetch<T>(this IGH_DataAccess da, int position) {
      
      T temp = default(T);
      da.GetData<T>(position, ref temp);
      return temp;
    }
    /// <summary>
    /// Fetch data with name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="da"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Fetch<T>(this IGH_DataAccess da, string name) {
      T temp = default(T);
      da.GetData<T>(name, ref temp);
      return temp;
    }

    /// <summary>
    /// Fetch data list with position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="da"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static List<T> FetchList<T>(this IGH_DataAccess da, int position) {
      List<T> temp = new List<T>();
      da.GetDataList<T>(position, temp);
      return temp;
    }

    /// <summary>
    /// Fetch data list with name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="da"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<T> FetchList<T>(this IGH_DataAccess da, string name) {
      List<T> temp = new List<T>();
      da.GetDataList<T>(name, temp);
      return temp;
    }
    /// <summary>
    /// Fetch structure with position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="da"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GH_Structure<T> FetchTree<T>(this IGH_DataAccess da, int position) where T : IGH_Goo {
      GH_Structure<T> temp;
      da.GetDataTree<T>(position, out temp);
      return temp;
    }

    /// <summary>
    /// Fetch structure with name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="da"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GH_Structure<T> FetchTree<T>(this IGH_DataAccess da, string name) where T : IGH_Goo {
      GH_Structure<T> temp;
      da.GetDataTree<T>(name, out temp);
      return temp;
    }
  }
}
