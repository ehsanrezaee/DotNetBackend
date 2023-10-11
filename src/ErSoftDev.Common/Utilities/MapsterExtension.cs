using Mapster;

namespace ErSoftDev.Common.Utilities
{
    public static class MapsterExtension
    {
        public static TTarget MapTo<TTarget>(this object source)
        {
            return source.Adapt<TTarget>();
        }

        public static TTarget MapTo<TSource, TTarget>(this TSource source, bool ignoreNullValue = false)
        {
            if (ignoreNullValue)
                TypeAdapterConfig<TSource, TTarget>
                    .NewConfig()
                    .IgnoreNullValues(true);
            return source.Adapt<TSource, TTarget>();
        }

        public static TTarget MapTo<TSource, TTarget>(this TSource source, TTarget destination, bool ignoreNullValue = false, int? maxDepth = null)
        {
            TypeAdapterConfig<TSource, TTarget>
                    .NewConfig()
                    .MaxDepth(maxDepth)
                    .IgnoreNullValues(ignoreNullValue)
                    .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
                    .Map(target => target, source1 => source1);
            return (TTarget)source.Adapt(destination, typeof(TSource), typeof(TTarget));
        }
    }
}
