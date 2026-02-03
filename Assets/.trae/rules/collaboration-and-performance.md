### 协作与性能 (Collaboration & Performance)
- 注释规范 ：
  - 所有公共类和核心方法必须包含 /// <summary> 文档注释。
  - 复杂的逻辑块必须有 // 行内注释。
  - 没有使用[LabelText]进行注释的变量，必须包含有 // 名称注释。
- 内存优化 ： 
  - 在 Update 中避免使用 GetComponent 或 GameObject.Find 。
  - 频繁生成的对象必须使用Unity的ObjectPool<T>对象池系统进行管理。