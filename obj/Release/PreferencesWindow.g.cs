﻿#pragma checksum "..\..\PreferencesWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B814DCFF7BD6A520CF06D25D82D210D6A40F1E8BC5E7442A972C045FE75145EE"
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
using System.Windows.Forms.Integration;
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
using UniversityTimetabling;


namespace UniversityTimetabling {
    
    
    /// <summary>
    /// PreferencesWindow
    /// </summary>
    public partial class PreferencesWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\PreferencesWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelTeacher;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\PreferencesWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBoxTeacher;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\PreferencesWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid GridMain;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\PreferencesWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle close;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\PreferencesWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelClose;
        
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
            System.Uri resourceLocater = new System.Uri("/UniversityTimetabling;component/preferenceswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PreferencesWindow.xaml"
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
            this.labelTeacher = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.comboBoxTeacher = ((System.Windows.Controls.ComboBox)(target));
            
            #line 11 "..\..\PreferencesWindow.xaml"
            this.comboBoxTeacher.Initialized += new System.EventHandler(this.comboBoxTeacher_Initialized);
            
            #line default
            #line hidden
            
            #line 11 "..\..\PreferencesWindow.xaml"
            this.comboBoxTeacher.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.comboBoxTeacher_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.GridMain = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.close = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 27 "..\..\PreferencesWindow.xaml"
            this.close.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.close_MouseUp);
            
            #line default
            #line hidden
            
            #line 27 "..\..\PreferencesWindow.xaml"
            this.close.KeyUp += new System.Windows.Input.KeyEventHandler(this.close_KeyUp);
            
            #line default
            #line hidden
            return;
            case 5:
            this.labelClose = ((System.Windows.Controls.Label)(target));
            
            #line 28 "..\..\PreferencesWindow.xaml"
            this.labelClose.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.close_MouseUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

