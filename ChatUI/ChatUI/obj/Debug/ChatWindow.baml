   M S B A M L   `   `   `  ����
  ChatUI    ChatUI.ChatWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
ChatWindow��5   	   $	��300��6   $	��350��6)   .��5      �  � RowDefinitions 5   
   �� 5      $��15 �6   �� 5      $	��85* �6   �� 5	      $	��15* �6    � ColumnDefinitions 5   
   �� 5      $
��200* �6   �� 5      $
��100* �6   .+�5   
   �� -   5        $��
ScrollChat��5      $��1��6)   $��2��66   .��5      �� -   5        $��textBoxChatPane��5   h    �� 
IsReadOnly .  �� TextWrapping$
 Wrap=�6(   $
��Auto��6<   $��Stretch=�6J   $��Stretch=�6   $
��Auto��6�   $��1��6�   $��0��6�   5      �� 5   
   -   5        $��textBoxEntryField��5   a   {�.  $
��Auto��6$   $��Stretch=�62   $��6,6,6,6q�6P   $��Stretch=�6z   $
��Auto��6�   $��2��6�   $��0��6�   6�   �� 5   
   -   5        $��sendBtn��5   `   {�.  $
��Auto��6#   $��Stretch=�61   $��6,6,6,6q�6O   $��Stretch=�6o   $
��Auto��6�   $��2��6�   $��1��6�   $
��Send��6�   6�   5      5      