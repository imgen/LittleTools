import 'package:flutter/material.dart';
import 'package:get/get.dart';

import '/services/theme_service.dart';
import '/base/base_appbar_view.dart';
import '{page_name}_controller.dart';

class {page_name}Page extends BaseAppBarView<XxxController> {
  final themeService = Get.find<ThemeService>();

  {page_name}Page({super.key});

  @override
  Widget body(BuildContext context) {
    
  }

  @override
  String get title => "{page_title}";
}
