//
// Copyright 2011 GREE International, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#define GREE_SYNTHESIZE_SINGLETON_FOR_CLASS(classname) \
+ (classname *)shared##classname:(classname *(^)(Class allocClass))initBlock \
{ \
    static dispatch_once_t predicate; \
    static classname *shared##classname; \
    dispatch_once(&predicate, \
    ^{ \
        classname *(^_initBlock)(Class); \
        _initBlock = (initBlock) ? initBlock : ^(Class allocClass) { return [[allocClass alloc] init]; }; \
        shared##classname = _initBlock(self); \
    }); \
    return shared##classname; \
}
