﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3371190FCA6A5FEEB04FA99EF79E166820494C74920659FE1E7EAD17EE2CAD27"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using _15puzzleWPF;


namespace _15puzzleWPF {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 30 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas TileCanvas;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DistanceText;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SearchDepthText;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TimeText;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NodesText;
        
        #line default
        #line hidden
        
        
        #line 119 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button LoadButton;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ShuffleButton;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SolveButton;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button AnimateButton;
        
        #line default
        #line hidden
        
        
        #line 190 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RichTextBox ConsoleBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/15puzzleWPF;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TileCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 36 "..\..\MainWindow.xaml"
            this.TileCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.MoveTilePressed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.DistanceText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.SearchDepthText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.TimeText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.NodesText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.LoadButton = ((System.Windows.Controls.Button)(target));
            
            #line 120 "..\..\MainWindow.xaml"
            this.LoadButton.Click += new System.Windows.RoutedEventHandler(this.LoadButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ShuffleButton = ((System.Windows.Controls.Button)(target));
            
            #line 136 "..\..\MainWindow.xaml"
            this.ShuffleButton.Click += new System.Windows.RoutedEventHandler(this.ShuffleButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 8:
            this.SolveButton = ((System.Windows.Controls.Button)(target));
            
            #line 153 "..\..\MainWindow.xaml"
            this.SolveButton.Click += new System.Windows.RoutedEventHandler(this.SolveButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.AnimateButton = ((System.Windows.Controls.Button)(target));
            
            #line 170 "..\..\MainWindow.xaml"
            this.AnimateButton.Click += new System.Windows.RoutedEventHandler(this.AnimateButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ConsoleBox = ((System.Windows.Controls.RichTextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

