namespace md2visio.struc.classdiag
{
    internal enum ClsRelationType
    {
        Inheritance,    // <|--  实心三角箭头 + 实线
        Composition,    // *--   实心菱形 + 实线
        Aggregation,    // o--   空心菱形 + 实线
        Association,    // -->   普通箭头 + 实线
        Dependency,     // ..>   普通箭头 + 虚线
        Realization,    // ..|>  空心三角箭头 + 虚线
        Link,           // --    无箭头实线
        DashedLink      // ..    无箭头虚线
    }

    internal enum ClsVisibility
    {
        Public,         // +
        Private,        // -
        Protected,      // #
        Internal        // ~
    }
}
