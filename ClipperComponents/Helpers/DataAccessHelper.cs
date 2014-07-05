using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StudioAvw.Tools {
  static class DataAccessHelper {


    /// <summary>
    /// Fetch data at index position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="DA"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    static public T Fetch<T>(this IGH_DataAccess DA, int position) {
      
      T temp = default(T);
      DA.GetData<T>(position, ref temp);
      return temp;
    }
    /// <summary>
    /// Fetch data with name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="DA"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    static public T Fetch<T>(this IGH_DataAccess DA, string name) {
      T temp = default(T);
      DA.GetData<T>(name, ref temp);
      return temp;
    }

    /// <summary>
    /// Fetch data list with position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="DA"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    static public List<T> FetchList<T>(this IGH_DataAccess DA, int position) {
      List<T> temp = new List<T>();
      DA.GetDataList<T>(position, temp);
      return temp;
    }

    /// <summary>
    /// Fetch data list with name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="DA"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    static public List<T> FetchList<T>(this IGH_DataAccess DA, string name) {
      List<T> temp = new List<T>();
      DA.GetDataList<T>(name, temp);
      return temp;
    }
    /// <summary>
    /// Fetch structure with position
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="DA"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    static public GH_Structure<T> FetchTree<T>(this IGH_DataAccess DA, int position) where T : IGH_Goo {
      GH_Structure<T> temp;
      DA.GetDataTree<T>(position, out temp);
      return temp;
    }

    /// <summary>
    /// Fetch structure with name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="DA"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    static public GH_Structure<T> FetchTree<T>(this IGH_DataAccess DA, string name) where T : IGH_Goo {
      GH_Structure<T> temp;
      DA.GetDataTree<T>(name, out temp);
      return temp;
    }
  }
}
