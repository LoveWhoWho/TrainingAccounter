﻿#pragma checksum "..\..\ViewWord.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F90EA1EAAD32A13D6B3E42F730032418"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
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


namespace TrainingAccounter {
    
    
    /// <summary>
    /// ViewWord
    /// </summary>
    public partial class ViewWord : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 77 "..\..\ViewWord.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSelect;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\ViewWord.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbxPathname;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\ViewWord.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnViewDoc;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\ViewWord.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAddcontent;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\ViewWord.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\ViewWord.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DocumentViewer documentviewWord;
        
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
            System.Uri resourceLocater = new System.Uri("/TrainingAccounter;component/viewword.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ViewWord.xaml"
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
            this.btnSelect = ((System.Windows.Controls.Button)(target));
            
            #line 77 "..\..\ViewWord.xaml"
            this.btnSelect.Click += new System.Windows.RoutedEventHandler(this.btnSelectWord_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbxPathname = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.btnViewDoc = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\ViewWord.xaml"
            this.btnViewDoc.Click += new System.Windows.RoutedEventHandler(this.btnViewDoc_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnAddcontent = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\ViewWord.xaml"
            this.btnAddcontent.Click += new System.Windows.RoutedEventHandler(this.btnAddcontent_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\ViewWord.xaml"
            this.btnExit.Click += new System.Windows.RoutedEventHandler(this.btnExit_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.documentviewWord = ((System.Windows.Controls.DocumentViewer)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

