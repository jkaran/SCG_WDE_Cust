   M S B A M L   `   `   `  ���� \  XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   AnimatedExpanderButtonTemp  �� 
TargetType ��Name �� Padding � ColumnDefinitions F�Name J� RenderTransformOrigin �� Angle ��Name �� Triggers	 P� Property
 P� Value N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions �� 
TargetName �� Property �� Value  StretchyExpanderTemp � RowDefinitions d�Name � OverridesDefaultStyle d� 	IsChecked �� Path �� RelativeSource � LayoutTransform �� ScaleY�� H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation       :x,http://schemas.microsoft.com/winfx/2006/xaml      %�  &	        &	 b    7   �� "  d�.R��� $ ExpanderButtonBorder��
�������# z��._�� $	��Top=� �� $��* ��� $��15 �.+��� 
������ $��2��A� $��Circle��$��1�����#17849D$��16��$��16��$��2��$��Center=�$��Center=�F� $ Arrow��$��1��"����@B `� ��g5  	= р�[ `� 	
�������$��2��$��Center=�$��Center=�$ 0.5,0.5&�z��� $ 0Q��� $ HeaderContent��$��0��$��4,0,0,0q�$��Center=�$��Content�� P� 	 �@d�	IsChecked$

 TrueY� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$	 180X�$ 0D� �� .���� .��W� $ Arrow��$4 .(Path.RenderTransform).(RotateTransform.Angle)�$ 0X�$ 0D�P� 	 �@J�IsMouseOver
 . . ��� $ Circle�� � ��
 �����P� 	 � ��
 . . ��� $ Circle�� � �� �#808080�� $ Arrow�� � ��
 ������� "  9�.R��� .+��  �� $��* ��� $��* � �� $��* �.+��� ���#17849D$��0��$��0��._�d� $ ExpanderButton��$��16��$��0��$��12(�$
��Bold"�
�������$��4q�8��  #��z�� . $��1.5,0q� �� $ 
IsExpanded� �� *TemplatedParent+�� $��
ExpContent�����#17849D
�������$��1q�$��0��$��1��._��� $ ExpanderContent��$��Content�� �� $ 0Q� P� 	 �@9�
IsExpanded
 .  �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$ 1X�$ 0D� �� .���� .��W� $ ExpanderContent��$@ :(ContentPresenter.LayoutTransform).(ScaleTransform.ScaleY)�$ 0X�$ 0D�. ��� $ 
ExpContent�� � ��$
 Auto��P� 	 �@9�
IsExpanded
 .  . ��� $ 
ExpContent�� � ��$ 0��