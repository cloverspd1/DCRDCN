namespace BEL.CommonDataContract
{
    using BEL.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Data Table Extension
    /// </summary>
    public static class DataTableExtensoins
    {
        /// <summary>
        /// To the model.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="table">The table.</param>
        /// <returns>model of type T</returns>
        public static T ToModel<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            T result = new T();

            if (table != null)
            {
                foreach (var row in table.Rows)
                {
                    var item = CreateItemFromRow<T>((DataRow)row, properties);
                    result = item;
                }
            }
            return result;
        }

        /// <summary>
        /// To the model.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="row">The row.</param>
        /// <returns>model of type T</returns>
        public static T ToModel<T>(this DataRow row) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            T item = new T();
            if (row != null)
            {
                foreach (var property in properties)
                {
                    string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                    if (row.Table.Columns.Contains(propertyName))
                    {
                        if (row[propertyName] != System.DBNull.Value)
                        {
                            object convertedValue = row[propertyName];

                            try
                            {
                                if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                                {
                                    Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                    convertedValue = (row[propertyName] == null || string.IsNullOrEmpty(Convert.ToString(row[propertyName]))) ? null : Convert.ChangeType(row[propertyName], t);
                                }
                                else if (property.PropertyType.IsPrimitive)
                                {
                                    convertedValue = System.Convert.ChangeType(row[propertyName], property.PropertyType);
                                }
                                else
                                {
                                    convertedValue = System.Convert.ChangeType(row[propertyName], property.PropertyType);
                                }
                            }
                            catch (InvalidCastException)
                            {
                                convertedValue = null;
                            }

                            property.SetValue(item, convertedValue, null);
                        }
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Converts Datatable into collection of T model.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="table">The table.</param>
        /// <returns>object of list type of T</returns>
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            if (table != null)
            {
                foreach (var row in table.Rows)
                {
                    var item = CreateItemFromRow<T>((DataRow)row, properties);
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// To the i list.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="table">The table.</param>
        /// <returns>list of T objects</returns>
        public static IList<T> ToIList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            if (table != null)
            {
                foreach (var row in table.Rows)
                {
                    var item = CreateItemFromRow<T>((DataRow)row, properties);
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Creates the item from row.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="row">The row.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>create T type of object</returns>
        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                string propertyName = property.GetCustomAttribute<FieldColumnNameAttribute>() != null && !string.IsNullOrEmpty(property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation) ? property.GetCustomAttribute<FieldColumnNameAttribute>().FieldsInformation : property.Name;
                if (row.Table.Columns.Contains(propertyName))
                {
                    if (row[propertyName] != System.DBNull.Value)
                    {
                        object convertedValue = row[propertyName];

                        try
                        {
                            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                            {
                                Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                convertedValue = (row[propertyName] == null || string.IsNullOrEmpty(Convert.ToString(row[propertyName]))) ? null : Convert.ChangeType(row[propertyName], t);
                            }
                            else if (property.PropertyType.IsPrimitive)
                            {
                                convertedValue = System.Convert.ChangeType(row[propertyName], property.PropertyType);
                            }
                            else
                            {
                                convertedValue = System.Convert.ChangeType(row[propertyName], property.PropertyType);
                            }
                        }
                        catch (InvalidCastException)
                        {
                            convertedValue = null;
                        }

                        property.SetValue(item, convertedValue, null);
                    }
                }
            }
            return item;
        }
    }
}