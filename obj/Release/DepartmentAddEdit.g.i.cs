﻿#pragma checksum "..\..\DepartmentAddEdit.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "EE35C75242F31B29107859A3AF718C9014B90A16"
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
    /// DepartmentAddEdit
    /// </summary>
    public partial class DepartmentAddEdit : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelName;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelFaculty;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtName;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox comboBox;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtId;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle saveDepartment;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle close;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\DepartmentAddEdit.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelSaveDepartment;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\DepartmentAddEdit.xaml"
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
            System.Uri resourceLocater = new System.Uri("/UniversityTimetabling;component/departmentaddedit.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DepartmentAddEdit.xaml"
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
            this.labelName = ((System.Windows.Controls.Label)(target));
            return;
            case 2:
            this.labelFaculty = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.txtName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.comboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 13 "..\..\DepartmentAddEdit.xaml"
            this.comboBox.Initialized += new System.EventHandler(this.comboBox_Initialized);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtId = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.saveDepartment = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 22 "..\..\DepartmentAddEdit.xaml"
            this.saveDepartment.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.saveDepartment_MouseUp);
            
            #line default
            #line hidden
            return;
            case 7:
            this.close = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 23 "..\..\DepartmentAddEdit.xaml"
            this.close.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.close_MouseUp);
            
            #line default
            #line hidden
            return;
            case 8:
            this.labelSaveDepartment = ((System.Windows.Controls.Label)(target));
            
            #line 24 "..\..\DepartmentAddEdit.xaml"
            this.labelSaveDepartment.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.saveDepartment_MouseUp);
            
            #line default
            #line hidden
            return;
            case 9:
            this.labelClose = ((System.Windows.Controls.Label)(target));
            
            #line 25 "..\..\DepartmentAddEdit.xaml"
            this.labelClose.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.close_MouseUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
