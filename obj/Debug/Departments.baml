   M S B A M L   `   `   `  ����P  LUniversityTimetabling, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:UniversityTimetablingUniversityTimetabling  5   	   '    !UniversityTimetabling.Departments   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:UniversityTimetabling     5   	   -   5          �Name$  departments��5   	    � Title$ Study programs��5   	   $	��500��6    $	��700��6-    � WindowStartupLocation$ CenterScreen=�6_    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�6�   .��5	      � .+�5
   
   &   System.Windows.Controls.DataGrid  -   5          Name$ departmentsGrid��5
         AutoGenerateColumns .     ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 6H      CanUserAddRows .  	   HorizontalGridLinesBrush	 �	#FF2C3C83
   VerticalGridLinesBrush
 �	#FF2C3C83   SelectionMode$ Single=�6�      
IsReadOnly . 
�������$��0,41,0,0q�6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6<      
Visibility$ Hidden=�6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *id+6H     5      $
 Name��6>    6%   �� *name+6>     5      $ Hidden=�6D    6%   �� *
faculty_id+6D     5      $ Faculty��6A    6%   �� *	faculty+6A   5      �� 5   
   -   5         ��Name$ addDepartment��5      
�������$
��Left=�68   $��25��6S   
���   �$	��Top=�6n   $��60��6�    �� RadiusY$	 8.5��6�    �� RadiusX$	 8.5��6�   $��13,7,0,0q�6�   6�   �� 5   
   -   5        $ editDepartment��5      
�������$
��Left=�69   $��25��6T   $��79,6,0,0q�6`   
���   �$	��Top=�6�   $��60��6�   $	 8.5��6�   $	 8.5��6�   6�   �� 5   
   -   5        $ deleteDepartment��5      
�������$
��Left=�6;   $��25��6V   $��	144,6,0,0q�6b   
���   �$	��Top=�6�   $��70��6�   $	 8.5��6�   $	 8.5��6�   6�   �� 5   
   -   5         ��Name$ labelAddDepartment��5      $	��Add��6,   $
��Left=�6:   $	��Top=�6U   $��35,7,0,0q�6m   $��24��6   $��38��6�   $��Verdana)�6�   $
��Bold"�6�   6�   �� 5   
   -   5        $ labelEditDepartment��5      $
��Edit��6-   $
��Left=�6<   $	��Top=�6W   $��	100,6,0,0q�6o   $��25��6�   $��38��6�   $��Verdana)�6�   $
��Bold"�6�   6�   �� 5   
   -   5        $ labelDeleteDepartment��5      $��Delete��6/   $
��Left=�6@   $	��Top=�6[   $��	162,6,0,0q�6s   $��27��6�   $��56��6�   $��Verdana)�6�   $
��Bold"�6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   z�5      V� 5      .~�5      �� �� 5      �� 5       �� Angle$ 0.252Q�6&   T� 5      5      5       �� 5!   
   -	   5        $ 
recAddIcon��5!      $
��Left=�6(   $��20��6C   
���   �$	��Top=�6^   $��20��6v   $��	16,10,0,0q�6�   $��0��6�   ��5"      �� 5#       �� ImageSource$ add.png��6    j� Stretch$ Uniform=�63   5%      �� 5&   
   -
   5        $ recEditIcon��5&      $
��Left=�6)   $��20��6D   $��82,9,0,0q�6P   
���   �$	��Top=�6q   $��20��6�   $��0��6�   ��5'      �� 5(      $ edit.png��6   $ Uniform=�64   5*      �� 5+   
   -   5        $ recIconDelete��5+      $
��Left=�6+   $��20��6F   
���   �$��0��6a   $	��Top=�6u   $��20��6�   $��	146,9,0,0q�6�   ��5,      �� 5-      $ 
delete.png��6   $ Uniform=�66   5/      �� 50   
   -   5        $ refreshDepartments��50      
�������$��25��6=   $��0,7,17,0q�6I   
���   �$	��Top=�6j   $	 8.5��6�   $	 8.5��6�   $��Right=�6�   $��85��6�   �� 51   
   -   5        $ labelRefreshDepartments��51      $��Refresh��61   $	��Top=�6C   $��0,7,21,0q�6[   $��27��6m   $��Verdana)�6y   $
��Bold"�6�   $ 0.5,0.5&�6�   $��Right=�6�   $��62��6  z�52      V� 53      .~�54      �� �� 55      �� 56      $ 0.252Q�6&   T� 57      58      5:      �� 5;   
   -   5        $ recIconRefresh��5;      $��15��6,   
���   �$��0��6G   $	��Top=�6[   $��	0,12,80,0q�6s   $��Right=�6�   $��15��6�   ��5<      �� 5=      $ refresh.png��6   $ Uniform=�67   5?      �� 5@   
   -   5        $ 	labelName��5@      $��Name:��6#   $
��Left=�63   $��	226,8,0,0q�6N   $	��Top=�6a   $��23��6y   $��62��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5A   
   -   5         ��Name$ txtFilterByName��5A      $
��Left=�6+   $��23��6F   $��	280,8,0,0q�6R    �� TextWrapping$
 Wrap=�6e   $�� ��6y   $	��Top=�6�   $	��151��6�    � ToolTip$ Filter By Name��6�   $ 0.414,0.475&�6�   6�   5B      5C      