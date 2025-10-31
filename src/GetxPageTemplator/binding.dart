import 'package:get/get.dart';

import '{page_name}_controller.dart';
import '{page_name}_provider.dart';

class {PageName}Binding extends Binding {
  @override
  List<Bind> dependencies() => [
        Bind.lazyPut<{PageName}Provider>(() => {PageName}Provider()),
        Bind.lazyPut<{PageName}Controller>(() => {PageName}Controller(Get.find())),
      ];
}
