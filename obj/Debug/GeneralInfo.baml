   M S B A M L   `   `   `  ����P  LUniversityTimetabling, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:UniversityTimetablingUniversityTimetabling  5   	   '    !UniversityTimetabling.GeneralInfo   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1i#clr-namespace:UniversityTimetabling     5   	   -   5          � Title$  General info��5   	   $	��450��6   $	��500��6+    � WindowStartupLocation$ CenterScreen=�6N    � ShowInTaskbar .   � 
ResizeMode$ NoResize=�6�   .��5	      � .+�5
   
   �� -   5         ��Name$ labelNrDays��5
      $��Number of days:��6%   $
��Left=�6?   $��
56,171,0,0q�6Z   $	��Top=�6n   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ comboBoxNrDays��5      $
��Left=�6+   $	��Top=�6F   $	��118��6^   $��322,171,0,0q�6j   
���������5      �� 5       �� EndPoint$	 0,1&�6&    �� 
StartPoint$	 0,0&�65   .��5      �  � Color$ 	#FFF0F0F0��6#   	 � Offset$	 0Q�65   � 5      $ White��6#   $	 1Q�61   5      5      �� 5   
   -   5        $ labelPeriodsperDay��5      $&�� Number of periods/hours per day:��6,   $
��Left=�6W   $��
56,211,0,0q�6r   $	��Top=�6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ cmbPeriodsperDay��5      $
��Left=�6-   $	��Top=�6H   $	��118��6`   $��322,211,0,0q�6l   
���������5      �� 5      $	 0,1&�6&   $	 0,0&�65   .��5      � $ 	#FFF0F0F0��6#   $	 0Q�65   � 5      $ White��6#   $	 1Q�61   5      5      �� 5   
   -   5        $ labelMinNrLecturesDay��5      $)��#Minimum number of lectures per day:��6/   $
��Left=�6]   $��
56,252,0,0q�6x   $	��Top=�6�   $��Verdana)�6�   $
��Bold"�6�   �� 5   
   -   5        $ cmbMinNrLecturesperDay��5      $
��Left=�63   $	��Top=�6N   $	��118��6f   $��322,252,0,0q�6r   
���������5      �� 5      $	 0,1&�6&   $	 0,0&�65   .��5       � $ 	#FFF0F0F0��6#   $	 0Q�65   � 5!      $ White��6#   $	 1Q�61   5"      5$      �� 5%   
   -   5        $ labelMaxNrLecturesDay��5%      $)��#Maximum number of lectures per day:��6/   $
��Left=�6]   $��
56,293,0,0q�6x   $	��Top=�6�   $��Verdana)�6�   $
��Bold"�6�   �� 5&   
   -	   5        $ cmbMaxNrLecturesperDay��5&      $
��Left=�63   $	��Top=�6N   $	��118��6f   $��322,293,0,0q�6r   
���������5'      �� 5(      $	 0,1&�6&   $	 0,0&�65   .��5)      � $ 	#FFF0F0F0��6#   $	 0Q�65   � 5*      $ White��6#   $	 1Q�61   5+      5-      A� 5.   
   $
��Left=�6   $	��150��6-   $��51,8,0,0q�6:   ���	#00000000$	��Top=�6_   $	��150��6w   ��5/      �� 50      
 j� Stretch$
 Uniform=�6    �� ImageSource$ 
upLogo.png��6/   52      �� 53   
   -
   5         ��Name$ saveGeneralInfo��53      
�������$
��Left=�6:   $��25��6U   
���   �$	��Top=�6p   $��60��6�    �� RadiusY$	 8.5��6�    �� RadiusX$	 8.5��6�   $��304,364,0,0q�6�    � Cursor$
 Hand��6�   |�. 6  �� 54   
   -   5        $ labelSaveGeneralInfo��54      $
��Save��6.   $
��Left=�6=   $��313,364,0,0q�6X   $	��Top=�6m   $��25��6�   $��51��6�   $��Verdana)�6�   $
��Bold"�6�   $
 Hand��6�   |�. 6�   �� 55   
   -   5        $ closeGeneralInfo��55      
�������$
��Left=�6;   $��25��6V   
���   �$	��Top=�6q   $��60��6�   $	 8.5��6�   $	 8.5��6�   $��374,364,0,0q�6�   $
 Hand��6�   |�. 6  �� 56   
   -   5        $ labelCloseGeneralInfo��56      $��Close��6/   $
��Left=�6?   $��381,364,0,0q�6Z   $	��Top=�6o   $��25��6�   $��57��6�   $��Verdana)�6�   $
��Bold"�6�   $
 Hand��6�   |�. 6�   �� 57   
   -   5        $ labelEditor��57      $��Timetable Editor:��6%   $
��Left=�6A   $��
56,331,0,0q�6\   $	��Top=�6p   $��Verdana)�6�   $
��Bold"�6�   �� 58   
   -   5         ��Name$ 	txtEditor��58      $
��Left=�6%   $��23��6@   $��205,331,0,0q�6L    �� TextWrapping$
 Wrap=�6a   $�� ��6u   $	��Top=�6}   $	��235��6�   59      5:      