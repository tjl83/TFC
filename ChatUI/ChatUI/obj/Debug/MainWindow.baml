   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   
 ChatUI   ChatUI.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=2.3.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   .s&clr-namespace:System;assembly=mscorlib  5   	     � Title$  TinFoil Chat��5       � WindowStartupLocation$ CenterScreen=�6   $	��280��5   	   $	��250��6   $	��280��5   	   $	��250��6   $	��280��5	   	   $	��250��6   .��5
      �  � RowDefinitions 5   
   �� 5      $	��150 �6   �� 5      $��50 �6   �� 5      $��50 �6   �� 5      $��50 �6   .+�5   
   �� $
��Left=�6   $��	10,0,0,30q�6/   $��1��6B   	Username:6Z   �� 5   
   $
��Left=�6   $��	41,25,0,5q�6/   $��1��6B   Port:6V   �� 5   
   -   5        $��Username��5      $��Right=�6"   $��	0,0,24,30q�6>   $��1��6Q   $	��139��6^   .��6j   Guest6q   %  Xceed.Wpf.Toolkit.MaskedTextBox  5   
   -   5          Name$
 Port��5      $��Right=�6+   $��	0,25,24,5q�6G   $��1��6Z   $	��139��6g      Mask$	 000��5      4  .Xceed.Wpf.Toolkit.Primitives.ValueRangeTextBox   ValueDataType" ��5      .��6>   4206C   �� 5   
   -   5         �� 	IsDefault . $��50,0,130,20q�5   "   $��2��6M   .��6Z   _Login6b   �� 5   
   -   5         �� IsCancel . $��130,0,50,20q�5   !   $��2��6C   .��6P   	_Cancel6Y   5      5      