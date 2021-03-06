﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

using CoreLocation;
using Foundation;

using BMapMain;

namespace Xamarin.Forms.BaiduMaps.iOS
{
    internal class PolylineImpl : BaseItemImpl<Polyline, BMKMapView, BMKPolyline>
    {
        protected override IList<Polyline> GetItems(Map map) => map.Polylines;

        protected override BMKPolyline CreateNativeItem(Polyline item)
        {
            CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[item.Points.Count];
            for (int i = 0; i < coords.Length; i++) {
                coords[i] = item.Points[i].ToNative();
            }

            BMKPolyline polyline = BMKPolyline.PolylineWithCoordinates(ref coords[0], (nuint)coords.Length);
            item.NativeObject = polyline;
            NativeMap.AddOverlay(polyline);

            ((INotifyCollectionChanged)(IList)item.Points).CollectionChanged += (sender, e) => {
                OnItemPropertyChanged(item, new PropertyChangedEventArgs(Polyline.PointsProperty.PropertyName));
            };

            return polyline;
        }

        protected override void UpdateNativeItem(Polyline item)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveNativeItem(Polyline item)
        {
            NativeMap.RemoveOverlay((NSObject)item.NativeObject);
        }

        protected override void RemoveNativeItems(IList<Polyline> items)
        {
            NSObject[] list = new NSObject[items.Count];
            for (int i = 0; i < items.Count; i++) {
                list[i] = (NSObject)items[i].NativeObject;
            }

            NativeMap.RemoveOverlays(list);
        }

        internal override void OnMapPropertyChanged(PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Polyline item = (Polyline)sender;
            BMKPolyline native = (BMKPolyline)item?.NativeObject;
            if (null == native) {
                return;
            }

            if (Polyline.TitleProperty.PropertyName == e.PropertyName) {
                native.Title = item.Title;
                return;
            }

            if (Polyline.PointsProperty.PropertyName == e.PropertyName) {
                CLLocationCoordinate2D[] points = new CLLocationCoordinate2D[item.Points.Count];
                for (int i = 0; i < points.Length; i++) {
                    points[i] = item.Points[i].ToNative();
                }

                native.SetPolylineWithCoordinates(ref points[0], points.Length);
                //NativeMap.MapForceRefresh(); // 没什么卵用
                var raw = NativeMap.CenterCoordinate;
                NativeMap.CenterCoordinate = new CLLocationCoordinate2D(
                    raw.Latitude, raw.Longitude + 0.0000000000000001
                );
            }
        }
    }
}

