using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace DrakeQuest.Data.Mapper
{
    public class MapperPropertyInfo : IMapperPropertyInfo
	{
		#region Properties

		public PropertyInfo PropertyInfo { get; }

		public string Name { get; protected set; }

		public int? Order { get; protected set; }

	    public object MetaInformation { get; protected set; }

        #endregion

        #region Constructor

        public delegate MapperPropertyInfo Factory(PropertyInfo propertyInfo);

        public MapperPropertyInfo(PropertyInfo propertyInfo, string name, int? order = null)
		{
			if (propertyInfo == null)
				throw new ArgumentNullException(nameof(propertyInfo));

			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			PropertyInfo = propertyInfo;
			Name = name;
			Order = order;
		}

		public MapperPropertyInfo(PropertyInfo propertyInfo)
		{
			PropertyInfo = propertyInfo;
			Initialize();
		}

		#endregion

		#region Methods

		protected virtual void Initialize(DataMemberAttribute attribute)
	    {
			Name = attribute.Name;
			Order = attribute.Order;
		}

		protected virtual void Initialize(ColumnAttribute attribute)
		{
			Name = attribute.Name;
		}

		protected virtual void Initialize(DisplayAttribute attribute)
		{
			Name = attribute.Name;
			Order = attribute.Order;
		}

		protected virtual void Initialize(PropertyDescriptionAttribute attribute)
		{
			Name = attribute.Name;
			Order = attribute.Order;
		}

	    protected virtual void Initialize<T>(T attribute)
		where T:Attribute
	    {
		    throw new NotImplementedException();
	    }

	    protected virtual void Initialize()
	    {
			object attribute = PropertyInfo.GetCustomAttributes(typeof (DataMemberAttribute), false).FirstOrDefault();

			if (SetMetaInformation<PropertyDescriptionAttribute>( p=> Initialize(p)))
				return;

			if (SetMetaInformation<DataMemberAttribute>(p => Initialize(p)))
				return;

			if (SetMetaInformation<ColumnAttribute>(p => Initialize(p)))
				return;

			if (SetMetaInformation<DisplayAttribute>(p => Initialize(p)))
				return;

			Name = PropertyInfo.Name;
	    }

	    protected bool SetMetaInformation<T>(Action<T> init)
		    where T : Attribute
	    {
			var attribute = (T) PropertyInfo.GetCustomAttributes(typeof (T), false).FirstOrDefault();
			if (attribute != null && init!= null)
			{
				init(attribute);
				MetaInformation = attribute;
			}
		    if (string.IsNullOrWhiteSpace(Name))
			    Name = PropertyInfo.Name;
		    return attribute != null;
	    }

		#endregion
	}
}
