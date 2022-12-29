using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    public partial class CategoryPartGroupBuilder : NopEntityBuilder<CategoryPartGroup>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CategoryPartGroup.CategoryId)).AsInt32().ForeignKey<Category>()
                .WithColumn(nameof(CategoryPartGroup.PartGroupId)).AsInt32().ForeignKey<PartGroup>();
        }

        #endregion
    }
}