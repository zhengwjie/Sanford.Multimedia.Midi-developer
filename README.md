## 《Windows Form实现MIDI音乐文件的播放APP》
      学院：软件学院  班级：4 班    学号：3017218181  姓名：郑万杰
      日期：   2019年  4月 4 日
# 一、功能概述
    对原有的Windows Form应用程序进行修改，原有项目可以播放单个Midi文件，现在可以播放多个文件，播放的方式可以由用户设定。
# 二、项目特色
    1.添加一个列表，可以在列表中显示Midi文件名
    2.用户可以通过单选按钮设定播放方式：单曲循环、列表循环、随机播放
    3.界面大小可以随意变化，在调整界面大小之后，界面内的控件不会跟窗体的比例大小不会变化。
# 三、代码总量
     自己编写的代码大概200行左右。
# 四、工作时间
     3天
# 五、运行截图
![](image/pig1.png)  
## 
     通过File->Open打开多个文件，将文件添加到播放列表中,单击其中的某个文件,就会自动播放。三个单选框用来设置列表中文件的播放顺序。新增Last跟Next两个按  钮，通过点击这两个按钮，切换正在播放的文件。
# 六、结论
    阅读别人的源码，找到自己不懂的地方，然后去理解其内在的实现机制，从这次实验中，理解最深刻的就是多线程、委托跟事件。
   
