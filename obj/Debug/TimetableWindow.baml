   M S B A M L   `   `   `  ����P  LUniversityTimetabling, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:UniversityTimetablingUniversityTimetabling  5   	   +    %UniversityTimetabling.TimetableWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:UniversityTimetabling     5   	     � Title$  Assign timetable��5       � WindowStartupLocation$ CenterScreen=�6    � WindowState$ 	Maximized=�6C    � ShowInTaskbar .   � Topmost .  � 
ResizeMode$ NoResize=�6�   .��5	      � -   5         �Name$	 win��5	      .+�5
   
   �� -   5         ��Name$ labelCourse��5
      $��Class:��6%   $
��Left=�66   $	��Top=�6Q   $��	20,10,0,0q�6i   $��23��6|   $��60��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ txtClass��5      $
��Left=�6$   $��23��6?   $��
101,10,0,0q�6K   	 �� TextWrapping$
	 Wrap=�6_   $�� ��6s   $	��Top=�6{   $	��289��6�   
 J� RenderTransformOrigin$
 0.414,0.475&�6�    �� 
IsReadOnly . 6�   �� 5   
   -   5        $ 
txtClassId��5      $y�Hidden=�6&   $�� ��6:    . 6B   �� 5   
   -   5        $ labelTeacher��5      $��Teacher:��6&   $
��Left=�69   $	��Top=�6T   $��	20,38,0,0q�6l   $��23��6   $��76��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ 
txtTeacher��5      $
��Left=�6&   $��23��6A   $��
101,38,0,0q�6M   $
	 Wrap=�6a   $�� ��6u   $	��Top=�6}   $	��289��6�   {�.  6�   �� 5   
   -   5        $ txtTeacherId��5      $y�Hidden=�6(   $�� ��6<    . 6D   �� 5   
   -   5        $ 	txtDetail��5      $y�Hidden=�6%   $�� ��69    . 6A   �� 5   
   -	   5        $ 	labelCode��5      $��Code:��6#   $
��Left=�63   $	��Top=�6N   $��	20,68,0,0q�6f   $��23��6y   $��76��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -
   5        $ txtCode��5      $
��Left=�6#   $��22��6>   $��
101,67,0,0q�6J   $
	 Wrap=�6^   $�� ��6r   $	��Top=�6z   $	��289��6�   {�.  6�   �� 5   
   -   5        $ txtSemester��5      $y�Hidden=�6'   $�� ��6;    . 6C   �� 5   
   -   5        $ txtLectureGroup��5      $y�Hidden=�6+   $�� ��6?    . 6G   �� 5   
   -   5        $ txtNumericGroup��5      $y�Hidden=�6+   $�� ��6?    . 6G   �� 5   
   -   5        $ txtLaboratoryGroup��5      $y�Hidden=�6.   $�� ��6B    . 6J   �� 5   
   -   5        $ txtDepartmentIds��5      $y�Hidden=�6,   $�� ��6@    . 6H   �� 5   
   -   5        $ labelLectures��5      $��	Lectures:��6'   $
��Left=�6;   $	��Top=�6V   $��	20,96,0,0q�6n   $��23��6�   $��76��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ txtNumberofLectures��5      $
��Left=�6/   $��23��6J   $��
101,94,0,0q�6V   $
	 Wrap=�6j   $�� ��6~   $	��Top=�6�   $	��289��6�   {�.  6�   �� 5   
   -   5        $ labelStudents��5      $��	Students:��6'   $
��Left=�6;   $	��Top=�6V   $��
20,125,0,0q�6n   $��23��6�   $��76��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ txtNumberofStudents��5      $
��Left=�6/   $��23��6J   $��101,124,0,0q�6V   $
	 Wrap=�6k   $�� ��6   $	��Top=�6�   $	��289��6�   {�.  6�   �� 5   
   -   5        $ labelDepartments��5      $��Study programs:��6*   $
��Left=�6D   $	��Top=�6_   $��
425,10,0,0q�6w   $��28��6�   $	��136��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ txbDepartments��5      $
��Left=�6-   $	��109��6H   $��
429,38,0,0q�6U   $
��Wrap=�6i   $�� ��6}   $	��Top=�6�   $	��316��6�   {�.  6�   �� 5   
   -   5        $ 
labelRooms��5      $#��Choose one of possible rooms:��6$   $
��Left=�6L   $	��Top=�6g   $��
791,10,0,0q�6   $��28��6�   $	��228��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ comboBoxRooms��5      $
��Left=�6*   $	��Top=�6E   $	��212��6]   $��
794,39,0,0q�6i   
���������5       �� 5!       �� EndPoint$	 0,1&�6&    �� 
StartPoint$	 0,0&�65   .��5"      �  � Color$ 	#FFF0F0F0��6#    � Offset$ 0Q�65   � 5#      $ White��6#   $ 1Q�61   5$      5&      �� 5'   
   -   5        $ 	labelInfo��5'      $��Currently assigned:��6#   $
��Left=�6A   $	��Top=�6\   $��
791,65,0,0q�6t   $��28��6�   $	��228��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5(   
   -   5        $ txbInfo��5(      $
��Left=�6&   $��25��6A   $��
795,94,0,0q�6M   $
��Wrap=�6a   $�� ��6u   $	��Top=�6}   $	��215��6�   {�.  6�   � 5)   
   -   5        $ GridMain��5)       � ShowGridLines .  $��	0,162,0,0q�6Q    � ColumnDefinitions 5*      �� 5+      $��* �6#    � RowDefinitions 5-      �� 5.      $��* �6    50      51      52      