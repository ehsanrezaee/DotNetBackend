using ErSoftDev.Framework.Configuration;
using IdGen;

namespace ErSoftDev.Framework.IdGenerate
{
    public class IdGenerator : IIdGenerator, IScopedDependency
    {
        public long CreateId()
        {
            var generator = new IdGen.IdGenerator(1);
            return generator.CreateId();
        }
    }
}
