using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    public class PartBuilder : NopEntityBuilder<Part>
    {
        #region Methods
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Part.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(Part.PartGroupId)).AsInt32().ForeignKey<PartGroup>();
        }

        #endregion
    }
}