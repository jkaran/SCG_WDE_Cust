   M S B A M L   `   `   `  ����\  XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   AnimatedExpanderButtonTemp  �� 
TargetType ��Name �� Padding � ColumnDefinitions F�Name J� RenderTransformOrigin �� Angle ��Name �� Triggers	 P� Property
 P� Value N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions �� 
TargetName �� Property �� Value  StretchyExpanderTemp � RowDefinitions d�Name � OverridesDefaultStyle d� 	IsChecked �� Path �� RelativeSource � LayoutTransform �� ScaleY�� 5      H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation       :x,http://schemas.microsoft.com/winfx/2006/xaml      5      %�  &	        &	 �    7   �� 5      "  d�5      .R�5   
   �� $ ExpanderButtonBorder��6   
�������# z��5
      ._�5      � $	��Top=�6    5      �� 5      $��* �6'   �� 5      $��15 �6'   .+�5      �� 
������ $��2��5   !   A� 5      $��Circle��6   $��1��5      ���#17849D$��16��5      $��16��5      $��2��5      $��Center=�5      $��Center=�5      F� 5      $ Arrow��6   $��1��5      "����@B `� ��g5  	= р�[ `� 	
�������$��2��5      $��Center=�5      $��Center=�5       $ 0.5,0.5&�5!      z�5"      �� 5#      $ 0Q�6*   5%      �� 5&      $ HeaderContent��6#   $��0��5'   '   $��4,0,0,0q�5(   '   $��Center=�68   $��Content��5)   '   5*      5+       5,   
   P� 5.      	 �@d�	IsChecked$

 TrueY�5/       50      �� 51      .��52      �� .��53      W� $ Arrow��6.   $4 .(Path.RenderTransform).(RotateTransform.Angle)�54   2   $	 180X�55   2   $ 0D�56   2   57      58       5:      �� 5;      .��5<      �� .��5=      W� $ Arrow��6.   $4 .(Path.RenderTransform).(RotateTransform.Angle)�5>   2   $ 0X�5?   2   $ 0D�5@   2   5A      5B      5D      P� 5G      	 �@J�IsMouseOver
 . . �5I      �� $ Circle��5K       � ��
 �����5L      P� 5M      	 � ��
 . . �5O      �� $ Circle��5Q       � �� �#808080�� 5R      $ Arrow��5T       � ��
 �����5U      5W      �� 5[      "  9�5\      .R�5]   
   �� .+�5^      �  5_      �� 5`      $��* �6$   61   �� 5a      $��* �6$   61    5c      �� 5d      $��* �6(   64   .+�5f      �� ���#17849D$��0��6.   $��0��6>   ._�5g      d� $ ExpanderButton��6#   $��16��6;   $��0��6G   $��12(�5h       $
��Bold"�6.   
�������$��4q�6S   8��  #��z��5j       . $��1.5,0q�5m       5k      �� $ 
IsExpanded� �� *TemplatedParent+5z      5{      �� 5|      $��
ExpContent��6B   ���#17849D
�������$��1q�6T   $��0��6h   $��1��6x   ._�5}      �� $ ExpanderContent��6'   $��Content��5~   #    5      �� 5�      $ 0Q�6-   5�      5�      5�      5�       5�   
   P� 5�      	 �@9�
IsExpanded
 .  5�      �� 5�      .��5�      �� .��5�      W� $ ExpanderContent��6.   $@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�5�   2   $ 1X�5�   2   $ 0D�5�   2   5�      5�       5�      �� 5�      .��5�      �� .��5�      W� $ ExpanderContent��6.   $@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�5�   2   $ 0X�5�   2   $ 0D�5�   2   5�      5�      . �5�      �� $ 
ExpContent��6    � ��$
 Auto��6C   6R   5�      P� 5�      	 �@9�
IsExpanded
 .  . �5�      �� $ 
ExpContent��6    � ��$ 0��6C   6O   5�      5�      5�      