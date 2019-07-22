using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StudioAvw.Clipper.Components.Helpers
{
    static class DataAccessHelper
    {


        /// <summary>
        /// Fetch data at index position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T Fetch<T>(this IGH_DataAccess da, int position)
        {

            var temp = default(T);
            da.GetData(position, ref temp);
            return temp;
        }
        /// <summary>
        /// Fetch data with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Fetch<T>(this IGH_DataAccess da, string name)
        {
            var temp = default(T);
            da.GetData(name, ref temp);
            return temp;
        }

        /// <summary>
        /// Fetch data list with position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static List<T> FetchList<T>(this IGH_DataAccess da, int position)
        {
            var temp = new List<T>();
            da.GetDataList(position, temp);
            return temp;
        }

        /// <summary>
        /// Fetch data list with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<T> FetchList<T>(this IGH_DataAccess da, string name)
        {
            var temp = new List<T>();
            da.GetDataList(name, temp);
            return temp;
        }
        /// <summary>
        /// Fetch structure with position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static GH_Structure<T> FetchTree<T>(this IGH_DataAccess da, int position) where T : IGH_Goo
        {
            da.GetDataTree(position, out GH_Structure<T> temp);
            return temp;
        }

        /// <summary>
        /// Fetch structure with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GH_Structure<T> FetchTree<T>(this IGH_DataAccess da, string name) where T : IGH_Goo
        {
            da.GetDataTree(name, out GH_Structure<T> temp);
            return temp;
        }
    }
}
