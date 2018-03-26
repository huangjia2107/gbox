using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System.IO;
using System.Reflection;

namespace DataAccess.Servers
{
    ///<summary>
    /// 该类返回一个SqlMapper,一个SqlMapper对应一个SqlMap.config配置文件
    ///</summary>
    class GetSqlMapper
    {
        private static ISqlMapper mapper=null;
        private GetSqlMapper() { }

        public static ISqlMapper GetMapper()
        {
            if (mapper == null)
            {
                Assembly myAssembly = Assembly.Load("DataAccess");

                using (Stream stream = myAssembly.GetManifestResourceStream("DataAccess.IBatisConfig.SqlMap.config"))
                {
                    DomSqlMapBuilder d = new DomSqlMapBuilder();//初始化一个DomSqlMapBuilder
                    mapper = d.Configure(stream);//调用Configure方法并指定配置文件的名称,返回一个SqlMapper 
                } 
                
            }

            return mapper;
        }
    }
}
