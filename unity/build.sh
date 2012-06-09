echo -e 'RoarAPI - unity (buidling...)'
echo -e '---------------------'

API_FUNCTION_JSON=../html5/libs/roarjs/api_functions.json
JSON_LINT="node ../html5/libs/roarjs/external/jsonlint"

echo -e "[Checking config json]"
cat "${API_FUNCTION_JSON}" | ${JSON_LINT} > /dev/null

echo -e "[Building unity from template]"
node template.js

echo -e "------"
echo -e "[Done]"
