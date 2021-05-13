# Tips:不要使用NuGet安装InteractiveDataDisplay库, 直接下载微软官方源码, 在Debug目录下的dll是可以在framework4.6.1下正常使用的

# 画动态旋转梯形
![image](https://user-images.githubusercontent.com/23237287/118134878-f95f3f00-b434-11eb-9c37-b539b044d501.png)
## 代码:
```Csharp
public void DrawFieldOfViewDynamic(LineGraph line,double center_X=0,double center_Y=0)
        {
           
            
            Task.Run(async () =>
            {
                //KS:构建原初图形 
                List<Point> pointList = new List<Point>();
                pointList.Add(new Point(-4, 6));
                pointList.Add(new Point(4, 6));
                pointList.Add(new Point(1, 1));
                pointList.Add(new Point(-1, 1));
                pointList.Add(new Point(-4, 6));

                for (double i = 0; i < 360; i += 2) //KS:i为精度 
                {
                    Thread.Sleep(100);
                    List<Point> pointListTrans = new List<Point>();
                    for (int j = 0; j < pointList.Count; j++)
                    {
                        Point tempPoint = RotateTransform(pointList[j].X - center_X, pointList[j].Y - center_Y, i);
                        pointListTrans.Add(tempPoint);
                        
                    }

                    //KS:切换两种刷新方法 
                    PenDrawKeepOld(line, pointListTrans);
                    //PenDrawRefresh(line, pointListTrans);
                }



                await Task.Delay(1);
            });
            
        }
```
