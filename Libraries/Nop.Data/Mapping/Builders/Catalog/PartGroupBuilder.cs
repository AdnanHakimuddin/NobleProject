using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    public class PartGroupBuilder : NopEntityBuilder<PartGroup>
    {
        #region Methods
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PartGroup.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(PartGroup.PartTypeId)).AsInt32().ForeignKey<PartType>();
        }

        #endregion
    }
}