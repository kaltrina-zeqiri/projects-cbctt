   M S B A M L   `   `   `  ����P  LUniversityTimetabling, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:UniversityTimetablingUniversityTimetabling  5   	   3    -UniversityTimetabling.CourseTeacherRelAddEdit   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:UniversityTimetabling     5   	     � Title$  Course teacher assignment��5   	   $	��195��6+   $	��445��68    � WindowStartupLocation$ CenterScreen=�6D    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�6   .��5	      � $��0,0,2,-3q�6   .+�5
   
   �� $
��Left=�6   $	��161��6/   
���   �$��0��6K   $	��Top=�6_   $	��166��6w   ���	#FFD2DEDE6�   �� 5   
   $
��Left=�6   $	��160��6/   $��1,1,0,0q�6<   
���   �$��0��6\   $	��Top=�6p   $	��165��6�   ��5      �� 5       j� Stretch$ Uniform=�6    �� ImageSource$ courseInstructors.png��6/   5      �� 5   
   -   5         ��Name$ labelCourse��5      $��Course:��6%   $
��Left=�67   $��
181,23,0,0q�6R   $	��Top=�6f   $��30��6~   $��73��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ labelTeacher��5      $��Teacher:��6&   $
��Left=�69   $��
181,63,0,0q�6T   $	��Top=�6h   $��30��6�   $��79��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ comboBoxCourse��5      $
��Left=�6+   $	��Top=�6F   $	��151��6^   $��
264,25,0,0q�6j   
������� �� 
IsEditable . ��5      �� 5      	 �� EndPoint$		 0,1&�6&   
 �� 
StartPoint$	
 0,0&�65   .��5      �  � Color$ 	#FFF0F0F0��6#    � Offset$ 0Q�65   � 5      $ White��6#   $ 1Q�61   5      5      �� 5   
   -   5        $ comboBoxTeacher��5      $
��Left=�6,   $	��Top=�6G   $	��151��6_   $��
264,65,0,0q�6k   
������� . ��5      �� 5      $		 0,1&�6&   $	
 0,0&�65   .��5      � $ 	#FFF0F0F0��6#   $ 0Q�65   � 5      $ White��6#   $ 1Q�61   5      5!      �� 5"   
   -   5         ��Name$ saveCourseTeacherRel��5"      
�������$
��Left=�6?   $��25��6Z   
���   �$	��Top=�6u   $��60��6�    �� RadiusY$	 8.5��6�    �� RadiusX$	 8.5��6�   $��284,116,0,0q�6�   |�. 6  �� 5#   
   -   5        $ close��5#      
�������$
��Left=�60   $��25��6K   
���   �$	��Top=�6f   $��60��6~   $	 8.5��6�   $	 8.5��6�   $��355,116,0,0q�6�   |�. 6�   �� 5$   
   -   5        $ labelSaveCourseTeacherRel��5$      $
��Save��63   $
��Left=�6B   $��293,116,0,0q�6]   $	��Top=�6r   $��25��6�   $��51��6�   $��Verdana)�6�   $
��Bold"�6�   6�   �� 5%   
   -   5        $ 
labelClose��5%      $��Close��6$   $
��Left=�64   $��362,116,0,0q�6O   $	��Top=�6d   $��25��6|   $��53��6�   $��Verdana)�6�   $
��Bold"�6�   6�   5&      5'      