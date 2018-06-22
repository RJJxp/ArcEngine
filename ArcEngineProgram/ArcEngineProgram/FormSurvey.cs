﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

namespace ArcEngineProgram
{
    public partial class FormSurvey : Form
    {
        bool flagSelectFeature = false;
        bool flagCreateFeature = false;

        ILayer pMovelayer;

        public FormSurvey()
        {
            InitializeComponent();
        }
        public DataTable GetLayerData(IFeatureLayer layer)
        {
            IFeature pFeature = null;
            DataTable pFeatDT = new DataTable();
            DataRow pDataRow = null;
            DataColumn pDataCol = null;
            IField pField = null;
            for (int i = 0; i < layer.FeatureClass.Fields.FieldCount; i++)
            {
                pDataCol = new DataColumn();
                pField = layer.FeatureClass.Fields.get_Field(i);
                pDataCol.ColumnName = pField.AliasName; //获取字段名作为列标题
                pDataCol.DataType = Type.GetType("System.Object");//定义列字段类型
                pFeatDT.Columns.Add(pDataCol); //在数据表中添加字段信息
            }
            IFeatureCursor pFeatureCursor = layer.Search(null, true);
            pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                pDataRow = pFeatDT.NewRow();
                //获取字段属性
                for (int k = 0; k < pFeatDT.Columns.Count; k++)
                {
                    pDataRow[k] = pFeature.get_Value(k);
                }

                pFeatDT.Rows.Add(pDataRow); //在数据表中添加字段属性信息
                pFeature = pFeatureCursor.NextFeature();
            }
            //释放指针
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
            //dataGridAttribute.BeginInit();
            return pFeatDT;

        }


        private void 加载Shp文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "shp文件(*.shp)|*.shp";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string fileFullPath = openFileDialog1.FileName;
            string pFolder = System.IO.Path.GetDirectoryName(fileFullPath);
            string pFileName = System.IO.Path.GetFileName(fileFullPath);
            axMapControl1.AddShapeFile(pFolder, pFileName);
            axMapControl1.Extent = axMapControl1.FullExtent;
            axMapControl1.Refresh();

            axMapControl2.AddShapeFile(pFolder, pFileName);
            axMapControl2.Extent = axMapControl1.FullExtent;
            axMapControl2.Refresh();

        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //获取当前的主窗体的坐标范围
            IEnvelope envelope = (IEnvelope)e.newEnvelope;
            //获取鹰眼视图画布容器
            IGraphicsContainer graphicsContatiner = axMapControl2.ActiveView.GraphicsContainer;
            graphicsContatiner.DeleteAllElements();
            //创建矩形元素
            IElement element = new RectangleElementClass();
            element.Geometry = envelope;
            //创建矩形轮廓的样式
            ILineSymbol linesymbol = new SimpleLineSymbolClass();
            linesymbol.Width = 2;

            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = 200;
            rgbColor.Green = 100;
            rgbColor.Blue = 0;
            rgbColor.Transparency = 255;
            linesymbol.Color = rgbColor;

            IFillSymbol fillsymbol = new SimpleFillSymbolClass();
            rgbColor.Transparency = 0;
            fillsymbol.Color = rgbColor;
            fillsymbol.Outline = linesymbol;

            IFillShapeElement fillshapeElement = element as IFillShapeElement;
            fillshapeElement.Symbol = fillsymbol;
            // 添加绘图元素

            graphicsContatiner.AddElement((IElement)fillshapeElement, 0);
            axMapControl2.Refresh();
        }

        private void 删除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            esriTOCControlItem pTocItem = new esriTOCControlItem();

            ILayer pLayer = new FeatureLayerClass();
            IBasicMap pBasicMap = new MapClass();
            object obj1 = new object();
            object obj2 = new object();
            axTOCControl1.GetSelectedItem(ref pTocItem, ref pBasicMap, ref pLayer, ref obj1, ref obj2);
            if (pTocItem == esriTOCControlItem.esriTOCControlItemLayer)
            {
                int iIndex;
                for (iIndex = 0; iIndex < axMapControl1.LayerCount; iIndex++)
                {
                    if (axMapControl1.get_Layer(iIndex) == pLayer)
                    {
                        axMapControl1.DeleteLayer(iIndex);
                        break;
                    }
                }
            }
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            this.axLicenseControl1.ContextMenuStrip = null;
            IBasicMap map = new MapClass();
            object other = null;
            object index = null;
            ILayer layer = new FeatureLayerClass();
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            try
            {
                this.axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            switch (e.button)
            {
                case 1:
                    if (item == esriTOCControlItem.esriTOCControlItemLayer)
                    {
                        if (layer is IAnnotationSublayer)
                            return;
                        else
                            pMovelayer = layer;
                        dataGridView1.DataSource = GetLayerData(layer as IFeatureLayer);
                    }
                    break;
                case 2://右键
                    System.Drawing.Point pt = new System.Drawing.Point(MousePosition.X, MousePosition.Y);
                    contextMenuStrip1.Show(pt);
                    break;
            }
        }

        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            if (e.button == 1 && pMovelayer != null)
            {
                esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                IBasicMap map = null;
                ILayer layer = null;
                object other = null;
                object index = null;
                axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                IMap pMap = axMapControl1.ActiveView.FocusMap;
                ILayer pTempLayer;
                int toindex = 0;
                if (item == esriTOCControlItem.esriTOCControlItemLayer && layer != null)
                {
                    if (pMovelayer != layer)
                    {
                        for (int i = 0; i < pMap.LayerCount; i++)
                        {
                            pTempLayer = pMap.get_Layer(i);
                            if (pTempLayer == layer)
                            {
                                toindex = i; ;
                            }
                        }
                        pMap.MoveLayer(pMovelayer, toindex);
                    }
                }

                axMapControl1.ActiveView.Refresh();
                axMapControl1.Update();
                pMovelayer = null;

            }
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {

        }
    }
}
