This is the history log file for the Unity Quick Dropdown package.

# 0.2.0

**New Features**
- Supported dropdown display for nested elements inside **List & Array**
- Added [FromAddressable] attribute.
- Added [FromConfig] attribute.
- Added a **Fix** button to create a new source if does not exist or recheck the source again after asset modifications.
- Warn about renamed sources in the console.

**Improvements**
- Huge performance optimization by reducing the frequency of asset lookup.
- Clicking on [FromFolder]'s source info will open the enclosing folder instead of just selecting it from outside.
- Draw the property as a normal field if the source does not exist.
- Undo support for ScriptableObject creation.
- Edit error status color. Use orange instead of red.

**Fixed Bugs**
- Prevent cyclic referencing which leads to stack overflow
- Prevent type mismatch error when using Dropdown fields with unsupported types

# 0.1.0
- Initial release