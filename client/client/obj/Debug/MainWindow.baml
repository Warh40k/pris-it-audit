   M S B A M L   `   `   `  ����A  =client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:clientclient  5   	   R NSystem.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J4clr-namespace:System.Data.OleDb;assembly=System.DataSystem.Data.OleDb 5   	       client.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:client     5   	   FOleDb4clr-namespace:System.Data.OleDb;assembly=System.Data     5   	     � Title$  
MainWindow��5	   	   $	��450��6   $	��800��6)   $��Stretch=�65   $	��800��6S   $	��450��6b   $��Stretch=�6r   $��Stretch=�6�    � 
ResizeMode$ CanResizeWithGrip=�6�    � WindowStartupLocation$ CenterScreen=�6�   .��5
      � .+�5   
   o� $��Stretch=�6   $	��800��64   $	��Top=�6C   $	��Top=�6b   $��18��6z   .@�5      m� -   5         m�Name$ sync_button��5      $ ��Синхронизация��6,   6C   5      ]� 5   
    J� RenderTransformOrigin$ 0.5,0.5&�6   $��	0,21,0,42q�64   .+�5      �� -   5         ��Name$ 	treeStack��5       �� Orientation$ Vertical=�6,   $	��140��6C   .+�5      �� -   5         ��Name$ 
tableLabel��5      $
��None��6,   $��Center=�6;   5      &   System.Windows.Controls.DataGrid  5      -   5          Name$ dataGrid��5      	   
IsReadOnly	 . $	��670��6;   $��Right=�6J   ���	#FFC6BCBC$��Stretch=�6x   $��Stretch=�6�   $��5,0,0,0q�6�   
   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$

 Auto 6�   5      � 5   
   -   5         �Name$ button_grid��5      $��0,0,0,0q�6$   $��Bottom=�65   $��0��6P   $��40��6]   {�.  .+�5      �� -   5         ��Name$ change_button��5      $��Изменить��6,   $��Center=�6?   $	��Top=�6c   $��Center=�6{   $��	70,10,0,0q�6�   $��60��6�   $��20��6�   6�   �� 5      -   5        $ 
add_button��5      $(��"Добавить
Добавить��6)   $��Center=�6N   $��	0,0,70,10q�6k   $��Bottom=�6~   $��60��6�   $��20��6�   6�   �� 5      -   5        $ delete_button��5      $��Удалить��6,   $��Center=�6>   $��
210,0,0,10q�6[   $��Bottom=�6o   $��60��6�   $��20��6�   6�   �� 5      -	   5        $ export_button��5      $��Экспорт��6,   $��Center=�6>   $��
600,0,0,10q�6[   $��Bottom=�6o   $��60��6�   $��20��6�   6�   5      5      5      