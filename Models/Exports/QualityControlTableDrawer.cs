using System;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class QualityControlTableDrawer : ContentDrawer
    {
        public QualityControlTableDrawer(IDrawingContext drawingContext,
                                         string saveFolderPath)
            : base(drawingContext, saveFolderPath)
        {
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
    }
}
