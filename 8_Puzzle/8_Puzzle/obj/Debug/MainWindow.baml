   M S B A M L   `   `   `  ����C  ?8_Puzzle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:_8_Puzzle	_8_Puzzle  5   	       _8_Puzzle.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:_8_Puzzle     5   	   -   5          � Title$  Puzzle Game��5   	   $	��525��6   $	��810��6*   .��5      �� -   5         ��Name$ canvas��5      .+�5   
   �� $��imgs/logo.png��6   $	��244��6'   $	��192��63    �� Left$ 33��6@   
 �� Top$	 -31��6Q   �� 5   
   $	 377��6   $ 13��6'   #��� ��67   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�62   .+�5      �� $��imgs/lighthouse.png��6   $��30��65   6C   �� 5      $��How To Play��6   $��Segoe UI Semibold)�6.   $��20(�6M   ���	#FFE51212$
��Bold"�6r   5      �� 5      $G��A1. Press button Play Game and choose the picture you want to play��6   $��Segoe UI Semibold)�6`   ���	#FF8B56566   �� 5      $.��(2. Use Arrow Key or Mouse to move pieces��6   $��Segoe UI Semibold)�6G   ���	#FF8B56566f   �� 5      $B��<3. If you finish the picture in time , you will win the game��6   $��Segoe UI Semibold)�6[   ���	#FF8B56566z   5      �� 5   
   	 �� X1$	 260��6   	 �� Y1$ 30��6   	 �� X2$	 260��6    	 �� Y2$	 330��6)   ���	#FF35A5DF$��4��6E   $	 131��6Y   $ 67��6j   6}   �� 5   
   $��Preview Image��6   $	 522��6(   $	 159��6:   $��Segoe UI Semibold)�6K   $��16(�6j   ���	#FF21437D���	#FF82F1976�   �� 5   
   -   5        	 ��Name$	 previewImage��5      �� 5   
   -   5        
 ��Name$
 btnPickPicture��5       J� RenderTransformOrigin$ 0.295,-0.038&�6I   $	 390��6n   $	 434��6�   $��Segoe UI Semibold)�6�   $��30��6�   $	��116��6�   .��5      �� $ 
Horizontal=�6   .+�5      �� $��imgs/play-station.png��6   $��20��67   $��20��6B   $��0,0,10,0q�6N   �� 5      $��16(�6   ���	#FF3B0374	Play Game6L   5       5!      �� 5"   
   -   5        $
 btnSaveGame��5"      $��30��6@   $	��116��6L   $	 656��6X   $	 434��6j   $��Segoe UI Semibold)�6{   .��5#      �� $ 
Horizontal=�6   .+�5$      �� $��imgs/diskette.png��6   $��20��63   $��20��6>   $��0,0,10,0q�6J   �� 5%      $��16(�6   ���	#FF3B0374	Save Game6L   5&      5'      �� 5(   
   -   5        $
 btnLoadGame��5(      $��30��6@   $	��116��6L   $	 525��6X   $	 434��6j   $��Segoe UI Semibold)�6{   .��5)      �� $ 
Horizontal=�6   .+�5*      �� $��imgs/puzzle.png��6   $��20��61   $��20��6<   $��0,0,10,0q�6H   �� 5+      $��16(�6   ���	#FF3B0374	Load Game6L   5,      5-      �� 5.   
   $ 
Horizontal=�6   $	 390��6.   $	 498��6?   ���	#FF97F04D.+�5/      �� $��imgs/countdown.png��6   $��20��60   �� 50      -   5         ��Name$ lbTime��50      $
��16px(�6$   $��Time Left: ��64   $ 5.884,11.859&�6J   $��Segoe UI Semibold)�6o   ���	#FF0806686�   51      �� 52   
   -   5        $ lbTimer��52      $��00:00��6!   $��20(�61   $ 22.924,1.104&�6?   $	 620��6d   $	 387��6v   $��Segoe UI Semibold)�6�   ���	#FFF306536�   �� 53   
   $
��mins��6   $	 681��6   $	 395��61   $��Segoe UI Semibold)�6B   54      55      