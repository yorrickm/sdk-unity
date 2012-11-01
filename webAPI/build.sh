echo -e 'RoarAPI - unity (buidling...)'
echo -e '---------------------'

API_FUNCTION_JSON=../api_functions.json
JSON_LINT=jsonlint

echo -e "[Checking config json]"
${JSON_LINT} "${API_FUNCTION_JSON}" > /dev/null

echo -e "[Building unity from template]"
node template.js

echo -e "------"
echo -e "[Done]"
