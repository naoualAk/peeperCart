   M S B A M L   `   `   `  ����U  QEmgu.CV.UI, Version=2.4.10.1939, Culture=neutral, PublicKeyToken=7281126722ab4438;,clr-namespace:Emgu.CV.UI;assembly=Emgu.CV.UI
Emgu.CV.UI  5   	   J FPepper Software, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:pepperSoft
pepperSoft 5   	      pepperSoft.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =emgu,clr-namespace:Emgu.CV.UI;assembly=Emgu.CV.UI     5   	   *localclr-namespace:pepperSoft     5   	     � Title$  Pepper Software��5
   	   $	��800��6!   $
��1000��6.    � Icon$0 */Pepper Software;component/bell-pepper.ico��6;    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ 
Style.xaml?�6%   %    5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��595* �6   �� 5      $��5 �6   �� 5      $
��380* �6    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $	��54* �6   �� 5      $��26 �6   �� 5       $
��124* �6   �� 5!      $
��177* �6   �� 5"      $��26 �6   �� 5#      $
��303* �6   �� 5%      $��30 �6   .+�5*   
   �� -   5         ��Name$ splitter��5*      $��1��6)   $��2��6;   $��5��6H   $��5��6Y   $��Stretch=�6c   o� 5-   
   $��1��6   $��3��6    .@�5.      m� $��_File��6   .@�5/      m� $��_Image��6   .@�50      m� -   5        $��	OpenImage��50      $��_Open image��60   {�.  6]   m� 51      -   5        $��
OpenFolder��51      $��_ Open folder��61   6H   52      m� 53      $��_Excel��6   {�.  .@�54      m� -   5        $��SvgExcel��54      $��_Sauvegarder��6/   6E   55      56      m� 57      $��_Image Processing��6   .@�58      m� $��_Segmentation��6   .@�59      m� -   5        $��_Create��59      60   m� 5:      -   5        $��_Apply��5:      6/   5;      m� 5<      $��_ Information Extraction��6   .@�5=      m� -   5        $��_Overall Analysis��5=      6:   m� 5>      -   5        $��_Count��5>      {�.  6E   m� 5?      -	   5        $��_Analyze��5?      {�.  6H   m� 5@      -
   5        $��_Analyze color��5@      {�.  6U   5A      m� 5B      $��_Display��6   .@�5C      m� -   5         m�Name$ affMask��5C      $��_Mask��60   	 m� IsCheckable	 . 6j   m� 5D      -   5        $ affKmean��5D      $��_Kmean��61   	 . 6m   5E      5F      m� 5G      -   5        $ acquisition_images��5G   4   $��_Acquisition Images��6   6P   5H      &   System.Windows.Controls.DataGrid  5J   
   -   5        
  Name$
 dataGridStats��5J   �      AlternatingRowBackground
 ����� #  Body_Content_DataGrid_Centering   	CellStyle# [  69   $��0q�6v    �� TextAlignment$ Center=�6�   $��2��6�   $��
10,0,10,10q�6�   $��3��6�      AutoGenerateColumns .    rowStyle   RowStyle# [ 6     GridLinesVisibility$
 None=�66  $��2��6Q     Columns 5K      0  *System.Windows.Controls.DataGridTextColumn  5L      ,  &System.Windows.Controls.DataGridColumn   Header$ Features name��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6<   �� *FeaturesName+  5M      $ Moyenne��6%    66   �� *Avg+ �� StringFormat$ {0:n2}��  5N      $ Standard��6%    67   �� *Std+$ {0:n2}��  5O      $ CV��6%    61   �� *CV+$ {0:n2}��  5P      $	 Max��6%    62   �� *Max+$ {0:n2}��  5Q      $	 Min��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6a    62   �� *Min+$ {0:n2}��6a   5S        5W   
   -   5        $
 dataGrid��5W   �   
 �����# [  6<   $��0q�6y   $ Center=�6�   $��2��6�   $��
10,0,10,10q�6�   $��6��6�    .  # [ 6  $
 None=�64  $��1��6O     
IsReadOnly . 6`  �� 5Z   
   -   5         ��Name$ listView��5Z      $��3��6%   $��
13,0,13,10q�62   $��1��6F   $��Disabled=�6r     FileItemStyle#��[ 5\       �� SelectionMode$ Single=�5]      ���	#FF5874A6$��Center=�6B   $	��576��6`   $��2��6�   ��5Z   Z   ��  �� SelectedItem 5[      �� *SelectedComputer+ �� RelativeSource ��  �� AncestorType$ WindowL���5^      �� 5_      .R�5`      � 5a      ��5d      �� 5e      .R�5f      ]� .+�5g      �� $��Bottom=�6$   $ Center=�6S   ��6<   �� *Name+6S   �� 5i      $��80��6    $��80��6,   $��0,2,0,0q�6W   ��68   �� *ImageSource+6W   5j      5k      5m      7  1System.Windows.Forms.Integration.WindowsFormsHost  5o   
    K� ZIndex$ 1��6   $��6��6,   $��
10,0,10,10q�6:   $��1��6N      Child. 5p         Emgu.CV.UI.ImageBox  -   5        
imgBox��5p      5q      �� 5t   
   $��Bottom=�6   $��30��6/   $��7��6;   $��3��6H   ��5u      �� 5v      .R�5w      �  5x      �� 5y      $	��150 �6/   �� 5z      $��2 �6/   �� 5{      $��3* �6/   �� 5|      $��2 �6/   �� 5}      $	��150 �6/   5      5�      .@�5�      �� .��5�      �� -   5        $
��info��5�      $��Version 1.0��6(   ���	#FF4D4D4D6;   5�      �� 5�      $��1��6   ���	#FF3432326(   �� 5�      $��2��6   $��Center=�6,   .��5�      �� -   5        $��info_mid��5�      ���	#FF385AB6$ Center=�6C   5�      �� 5�      $��3��6   �� 5�      $��4��6   .��5�      � -   5         �Name$ progressBar��5�       � Maximum$	 100Q�64   $	��150��6B   $��16��6N   $��Center=�6Z   $y�Hidden=�6w   5�      5�      �� 5�   
   -   5          ��Name$  	labelDoss��5�      $	��576��6   $��Folder Images��6/   $��	15,0,11,0q�6H   $��2��6[   $��Center=�6h   $��Stretch=�6�   ���	#FF4FB3E2���	#FFF7F6EC6�   �� 5�   
   -   5        $  labelStatsGlobale��5�      $��Aggregate statistics��6+   $��	10,0,10,0q�6K   $��2��6^   $��2��6k   $��Center=�6{   $��Stretch=�6�   ���	#FF4FB3E2���	#FFF7F6EC6�   �� 5�   
   -   5        $  
labelStats��5�      $��
Statistics��6$   $��	10,0,10,0q�6:   $��5��6M   $��2��6Z   $��Center=�6j   $��Stretch=�6�   ���	#FF4FB3E2���	#FFF7F6EC6�   �� 5�   
   -   5        $  labelImgBox��5�      $��Image��6%   $��	10,0,10,0q�66   $��5��6I   $��0��6V   $��Center=�6f   $��Stretch=�6�   ���	#FF4FB3E2���	#FFF7F6EC6�   5�      5�      