﻿#pragma checksum "..\..\..\Vue\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "97D0BAE24F177A35341FBF3D8F257B9E02380F31"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using SegmenTool;
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


namespace SegmenTool {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel stackPanel;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_segmentation;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_segmentation;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_input_folder;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_input_folder;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox_output_folder;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_output_folder;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_process;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock info;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock info_mid;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\Vue\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar progressBar;
        
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
            System.Uri resourceLocater = new System.Uri("/SegmenTool;component/vue/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Vue\MainWindow.xaml"
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
            this.stackPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.textBox_segmentation = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.button_segmentation = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\..\Vue\MainWindow.xaml"
            this.button_segmentation.Click += new System.Windows.RoutedEventHandler(this.button_segmentation_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.textBox_input_folder = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.button_input_folder = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\Vue\MainWindow.xaml"
            this.button_input_folder.Click += new System.Windows.RoutedEventHandler(this.button_input_folder_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.textBox_output_folder = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.button_output_folder = ((System.Windows.Controls.Button)(target));
            
            #line 46 "..\..\..\Vue\MainWindow.xaml"
            this.button_output_folder.Click += new System.Windows.RoutedEventHandler(this.button_output_folder_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.button_process = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\Vue\MainWindow.xaml"
            this.button_process.Click += new System.Windows.RoutedEventHandler(this.button_process_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.info = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.info_mid = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.progressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
