   M S B A M L   `   `   `  ����P  LUniversityTimetabling, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:UniversityTimetablingUniversityTimetabling  5   	   9    3UniversityTimetabling.CoursePeriodPreferenceAddEdit   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5local#clr-namespace:UniversityTimetabling     5   	     � Title$#  CoursePeriodPreferenceAddEdit��5   	   $	��210��6/   $	��445��6<    � WindowStartupLocation$ CenterScreen=�6H    � 
ResizeMode$ NoResize=�6m   .��5	      � $��0,0,2,-3q�6   .+�5
   
   �� $
��Left=�6   $	��160��6/   
���   �$��0��6K   $	��Top=�6_   $	��165��6w   $��4,8,0,0q�6�   ��5      �� 5       j� Stretch$ Uniform=�6    �� ImageSource$ periodPreferences.png��6/   5      �� 5   
   -   5         ��Name$ labelCourse��5      $��Course:��6%   $
��Left=�67   $��
175,23,0,0q�6R   $	��Top=�6f   $��30��6~   $��73��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ labelDay��5      $
��Day:��6"   $
��Left=�61   $��
175,63,0,0q�6L   $	��Top=�6`   $��30��6x   $��79��6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ labelPeriod��5      $��Unavailable 
periods:��6%   $
��Left=�6N   $��
174,94,0,0q�6i   $	��Top=�6}   $��39��6�   $��90��6�   $��Verdana)�6�   $
��Bold"�6�    J� RenderTransformOrigin$ 0.561,2.088&�6�   �� 5   
   -   5         ��Name$ comboBoxCourse��5      $
��Left=�6+   $	��Top=�6F   $	��151��6^   $��
264,25,0,0q�6j   
������� �� 
IsEditable . ��5      �� 5      	 �� EndPoint$		 0,1&�6&   
 �� 
StartPoint$	
 0,0&�65   .��5      �  � Color$ 	#FFF0F0F0��6#    � Offset$ 0Q�65   � 5      $ White��6#   $ 1Q�61   5      5      �� 5   
   -   5        $ comboBoxDay��5      $
��Left=�6(   $	��Top=�6C   $	��151��6[   $��
264,65,0,0q�6g   
���������5      �� 5      $		 0,1&�6&   $	
 0,0&�65   .��5      � $ 	#FFF0F0F0��6#   $ 0Q�65   � 5      $ White��6#   $ 1Q�61   5      5!      �� 5"   
   -   5        $ comboBoxPeriod��5"      $
��Left=�6+   $	��Top=�6F   $	��151��6^   $��264,102,0,0q�6j   
���������5#      �� 5$      $		 0,1&�6&   $	
 0,0&�65   .��5%      � $ 	#FFF0F0F0��6#   $ 0Q�65   � 5&      $ White��6#   $ 1Q�61   5'      5)      �� 5*   
   -   5         ��Name$! saveCoursePeriodPreferences��5*      
�������$
��Left=�6F   $��25��6a   
���   �$	��Top=�6|   $��60��6�    �� RadiusY$	 8.5��6�    �� RadiusX$	 8.5��6�   $��284,138,0,0q�6�   6�   �� 5+   
   -   5        $ close��5+      
�������$
��Left=�60   $��25��6K   
���   �$	��Top=�6f   $��60��6~   $	 8.5��6�   $	 8.5��6�   $��355,138,0,0q�6�   6�   �� 5,   
   -	   5        $&  labelSaveCoursePeriodPreferences��5,      $
��Save��6:   $
��Left=�6I   $��293,138,0,0q�6d   $	��Top=�6y   $��25��6�   $��51��6�   $��Verdana)�6�   $
��Bold"�6�   6�   �� 5-   
   -
   5        $ 
labelClose��5-      $��Close��6$   $
��Left=�64   $��362,138,0,0q�6O   $	��Top=�6d   $��25��6|   $��53��6�   $��Verdana)�6�   $
��Bold"�6�   6�   5.      5/      